Switcheroo version 0.4.1
----------------------

Copyright 2009, 2010 James Sulak
jsulak@gmail.com

This is an ALPHA release.  I've been happily using it in my day-to-day
work, but there are still bugs and hasn't been thoroughly tested
across platforms.

Usage:

The default key binding for Switcheroo is Win+w.  Pressing this key
combination will bring up the Switcheroo window, which lists the titles
of every open window.  Typing filters the list.  Press enter to switch
to the selected window.  Press Ctrl-enter to close the selected window.

The key binding can be changed by right-clicking the notification
icon. 


License:

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

For a copy of the license, see <http://www.gnu.org/licenses/>.


Credits:

The program icon was created by Mark James and is a part of the Silk icon
set 1.3.  It is available at: <http://www.famfamfam.com/lab/icons/silk/>.

This program uses the Managed Windows API to establish its hotkey
bindings.  It's licensed under the GNU LGPL and is available at
<http://mwinapi.sourceforge.net/>. 


Changelog:

2010-05-03: Released bugfix v0.4.1
  * Long windows titles are now truncated.
2010-02-07: Released v0.4
  * Window now resizes to match height and width of all entries
  * Window exception list is now user-editable.  
  * Tested on 32-bit Windows 7.
2009-11-09: Released v0.3
  * Added ctrl-enter functionality.
  * Mostly migrated to using the Managed Windows API instead of custom 
    window class.
2009-11-01: Released v0.2
2009-10-11: Released v0.1

