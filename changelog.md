Change log
----------

### 2015-01-15: v0.8.3 ###
- Crashes on launch in Windows 10 or when .NET 4.6 Preview is installed (#20)

### 2014-10-15: v0.8.2 ###
- Use icons from the taskbar (#19)

### 2014-10-15: v0.8.1 ###
- Fix crash when opening the Options window while the hotkey is already in use (#18)

### 2014-09-03: v0.8.0 ###
- Activate Switcheroo instead of the native task switcher with Alt+Tab [You need to enable this feature under Options] (#16)
- Option whether to start Switcheroo automatically on startup or not (#3)
- Ensure that the input field has a minimum width (#1)
- Remember key bindings and other settings when upgrading (#14)
- The Windows included are closer to those in the native Alt+Tab task switcher (#17)

### 2014-04-18: v0.7.3 ###
- Portable version of Switcheroo (#10)
- Icons are now shown for admin processes as well (#11)
- Decrease flickering when closing a window with CTRL+Enter (#12)

### 2014-03-04: v0.7.2 ###
- New Switcheroo icon
- Allow circling in the window list (@ovesen)
- Align filtering and highlighting algorithms
- Fix crash when pressing key up or down while the window list is empty (@ovesen)
- Fix potential crash in update check

### 2014-01-30: v0.7.1 ###
- Fix crash if process icon could not be found

### 2014-01-24: v0.7 ###
- Faster load time and filtering
- Grabs focus right away
- Highlights matching characters
- Included windows should be closer to the default alt+tab
- Informs you when a new version of Switcheroo is available
- Requires .NET 4.5

### 2014-01-13: v0.6 ###
- Development continued by Regin Larsen
- Shows process icon and process title in addition to window title
- No window chrome
- Simple scoring algorithm when filtering
- Support for ReSharper like filtering, e.g. hc for HipChat
- New default key binding `Alt + Space` (Windows 8 is using `Win + W`)

### 2010-07-18: v0.5 ###
- Hotkey now hides Switcheroo window in addition to showing it (Issue 4)
- Double-clicking on item now activates that window (Issue 4)
- Added mutex to ensure only one instance is running
- Attempted bugfix of Windows 7 64-bit window-switching bug (Issue 3).

### 2010-05-03: v0.4.1 ###
- Long windows titles are now truncated.

### 2010-02-07: v0.4 ###
- Window now resizes to match height and width of all entries
- Window exception list is now user-editable.  
- Tested on 32-bit Windows 7.

### 2009-11-09: v0.3 ###
- Added ctrl-enter functionality.
- Mostly migrated to using the Managed Windows API instead of custom window class.

### 2009-11-01: v0.2 ###

### 2009-10-11: v0.1 ###
