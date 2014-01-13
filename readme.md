Switcheroo
==========

*The humble incremental-search task switcher for Windows.*

![Swictheroo](https://github.com/kvakulo/Switcheroo/raw/master/screenshot.png)

Switcheroo is for anyone who spends more time using a keyboard than a mouse.
Instead of alt-tabbing through a (long) list of open windows, Switcheroo allows
you to quickly switch to any window by typing in just a few characters of its title.
It’s inspired by Emacs’s IDO mode buffer switching.

Download
--------
Download the [installer](https://github.com/downloads/jsulak/Switcheroo/Switcheroo_v0.5_setup.exe).

Usage
-----

The default key binding for Switcheroo is `Alt + Space`.  Pressing this key
combination will bring up the Switcheroo window, which lists the titles
of every open window.  Typing filters the list.  Press enter to switch
to the selected window.  Press `Ctrl + Enter` to close the selected window.

The key binding can be changed by right-clicking the notification icon. 

By default, Switcheroo excludes certain “windows” from the list, for example,
“Start,” which represents the Start Menu in Windows 7. This list can be edited
from the options dialog.

Credits
-------

The program icon was created by Mark James and is a part of the Silk icon
set 1.3.  It is available at: <http://www.famfamfam.com/lab/icons/silk/>.
 
This program uses the Managed Windows API to establish its hotkey
bindings.  It’s licensed under the GNU LGPL and is available at
<http://mwinapi.sourceforge.net/>. 

License
-------

Copyright 2014 Regin Larsen

Copyright 2009, 2010 James Sulak

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

For a copy of the license, see <http://www.gnu.org/licenses/>.

Change log
---------

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

