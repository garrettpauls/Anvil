using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using Anvil.Framework.Reactive;
using Anvil.Services.Data;

namespace Anvil.Services
{
    public interface IConfigurationService : IService
    {
        T GetValue<T>(string key, Func<T> defaultValueFactory);

        void SetValue<T>(string key, T value);
    }

    /// <remarks>
    ///     These methods really *should* return tasks, but most usages can't take advantage of it (e.g. used in properties)
    ///     and using tasks greatly complicated threadsafe caching. Generally operations are fast enough for it not to be
    ///     necessary, so we don't worry about it.
    /// </remarks>
    public sealed class ConfigurationService : DisposableReactiveObject, IConfigurationService
    {
        private readonly ConcurrentDictionary<string, object> mConfigCache =
            new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        private readonly IFormatter mFormatter = new BinaryFormatter();
        private readonly ISqlService mSql;

        public ConfigurationService(ISqlService sql)
        {
            mSql = sql;
        }

        private async Task<T> _GetValueFromDatabase<T>(string key, Func<T> defaultValueFactory)
        {
            var configData =
                (await mSql.Run(sql => sql.QueryAsync(
                    $"SELECT Value, Type FROM Configuration WHERE [Key] = {key} LIMIT 1",
                    async row => new
                    {
                        Value = await row.GetValueAsync<byte[]>(0),
                        Type = await row.GetValueAsync<string>(1)
                    }))).FirstOrDefault();

            T data;
            if(configData == null)
            {
                data = defaultValueFactory();
                await _SetValueInDatabase(key, data);
            }
            else
            {
                var type = Type.GetType(configData.Type, true);
                if(!typeof(T).IsAssignableFrom(type))
                {
                    throw new NotSupportedException($"Cannot deserialize config value {key}, stored type {type.AssemblyQualifiedName} is not assignable to requested type {typeof(T).AssemblyQualifiedName}");
                }

                object deserialized;
                using(var stream = new MemoryStream(configData.Value))
                {
                    deserialized = mFormatter.Deserialize(stream);
                }

                data = (T) Convert.ChangeType(deserialized, typeof(T));
            }

            return data;
        }

        private byte[] _Serialize(object value)
        {
            if(value == null)
            {
                return null;
            }

            using(var data = new MemoryStream())
            {
                mFormatter.Serialize(data, value);
                return data.ToArray();
            }
        }

        private Task _SetValueInDatabase<T>(string key, T value)
        {
            var type = value?.GetType() ?? typeof(object);
            if(!type.IsSerializable)
            {
                throw new NotSupportedException($"Cannot set configuration key {key} to value: type {type.FullName} is not serializable.");
            }

            var serializedValue = _Serialize(value);
            return mSql.Run(sql => sql.ExecuteAsync($@"
INSERT OR REPLACE INTO Configuration ([Key], Value, Type)
                              VALUES ({key}, {serializedValue}, {type.AssemblyQualifiedName})
"));
        }

        public T GetValue<T>(string key, Func<T> defaultValueFactory)
        {
            return (T) mConfigCache.GetOrAdd(key, _ => _GetValueFromDatabase(key, defaultValueFactory).Result);
        }

        public void SetValue<T>(string key, T value)
        {
            mConfigCache.AddOrUpdate(
                key,
                _ =>
                {
                    _SetValueInDatabase(key, value).Wait();
                    return value;
                },
                (_, old) =>
                {
                    _SetValueInDatabase(key, value).Wait();
                    return value;
                });

            mConfigCache.AddOrUpdate(key, value, (_, old) => value);
        }
    }
}
