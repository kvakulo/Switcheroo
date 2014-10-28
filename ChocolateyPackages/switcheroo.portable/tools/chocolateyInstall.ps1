$packageName = 'switcheroo.portable'
$url = 'https://github.com/kvakulo/Switcheroo/releases/download/v0.8.2/switcheroo-portable.zip'

Install-ChocolateyZipPackage "switcheroo" "$url"  "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"