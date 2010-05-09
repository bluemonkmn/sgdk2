@echo off
echo [ Scrolling Game Development Kit 2.0                            ]
echo [                                                               ]
echo [ Copyright (c) 2000-2008 Benjamin Marty                        ]
echo [ (BlueMonkMN@users.sourceforge.net)                            ]
echo [                                                               ]
echo [ Distributed under the GNU General Public License (GPL)        ]
echo [  - see included file COPYING.txt for details, or visit:       ]
echo [    http://www.fsf.org/copyleft/gpl.html                       ]

echo NOTE: You must acquire OpenTK.dll in order to build this project.
echo       See http://opentk.sf.net/ and put OpenTK.dll in SGDK2IDE\.

%windir%\Microsoft.NET\Framework\v3.5\MSBuild.exe /p:Configuration=Release
