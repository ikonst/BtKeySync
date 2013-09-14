BtKeySync
=========

On a dual-boot machine, when you pair a Bluetooth device with one OS, it wouldn't
be automatically paired with the other OS. Worse yet, if the other OS already had
the pairing set up, it will lose it. This project solves this issue so that if you
pair one OS, the others would be seamlessly paired.

Installation
============

Use InstallUtil.exe to install.

TODO
====

* Re-implement in C or C++ to make it lightweight.

* Make the Windows service self-installable (remove dependency on InstallUtil).

* Provide prebuilt binaries.

* Moving pairing from Windows to OS X. The best way about it would probably be
  writing an OS X utility that reads the NT registry hive. OS X comes with
  a read-only NTFS driver.
  
* Moving pairing from Linux to Windows, assuming an ext3 fs driver is installed.

* Moving pairing from Windows to Linux.
