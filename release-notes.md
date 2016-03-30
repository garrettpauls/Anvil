### vNext

* Added functionality to copy/paste environment variables.

### v0.0.1.10

* Migrated application configuration storage to the database instead
  of the app.config file for consistent configuration between versions.

### v0.0.1.9

* Added a balloon notification when a program fails to launch.
* Automatically upgrades app settings when moving to a new version.

### v0.0.1.8

* Added a settings menu.
* Added setting to use pre-release updates or not.
* Added setting to close to the system tray when main window closes.

### v0.0.1.7

* Added forced creation of logs directory as NLog doesn't reliably create it.
* Fixed issue #4. The launcher path and working directory browse buttons now update the value correctly.

### v0.0.1.6

* Updated environment variable support to expand variables as much as possible
  before launching a program.
* Environment variables in the program path and working directory are also
  expanded before launch.

### v0.0.1.5

* Replaced system tray context menu with WPF-style menu.
* Added ability to launch items from system tray menu.

### v0.0.1.4

* Changed update process to download releases before applying them.

### v0.0.1.3

* Added border to main window.
* Added scroll bar to environment variable collection editor.

### v0.0.1.2

* Improved error handling when checking for updates.
* Updated default logging to allow creating directories for log files.

### v0.0.1.1

* Updated UI to use MahApps metro styles.
* Changed application to automatically check for updates (this may break automatic updating, so manually check for updates if you don't see any in the future).

### v0.0.1.0

* This is a pre-release version with incomplete functionality.
