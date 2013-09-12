using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Win32;
using System.ServiceProcess;

namespace BtKeySync
{
    public class BtKeySync : ServiceBase
    {
        public BtKeySync()
        {
        }

        protected override void  OnStart(string[] args)
        {
            base.OnStart(args);

            // Open the system-wide link keys registry key.
            // This registry key is accessible to SYSTEM only, so run this process as a service.
            RegistryKey keysRegKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\services\BTHPORT\Parameters\Keys");

            // Import link keys from mounted Mac partitions
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                string bluedPlistPath = Path.Combine(di.RootDirectory.FullName, @"private\var\root\Library\Preferences\blued.plist");

                if (!File.Exists(bluedPlistPath))
                    continue;

                IDictionary<string, object> macBt = (IDictionary<string, object>)
                    PlistCS.Plist.readPlist(bluedPlistPath);

                var linkKeys = (IDictionary<string, object>) macBt["LinkKeys"];
                foreach (KeyValuePair<string, object> linkKey in linkKeys)
                {
                    string deviceMacAddress = linkKey.Key.Replace("-", "");

                    RegistryKey deviceRegKey = keysRegKey.CreateSubKey(deviceMacAddress);

                    var keys = (IDictionary<string, object>)linkKey.Value;

                    foreach (KeyValuePair<string, object> key in keys)
                    {
                        string keyStr = key.Key.Replace("-", "");;
                        byte[] keyBin = ((byte[])key.Value).Reverse().ToArray();

                        object existingKeyValue = deviceRegKey.GetValue(keyStr);
                        if (existingKeyValue == null)
                            EventLog.WriteEntry(string.Format("Device {0} key {1} does not exist, adding.", deviceMacAddress, keyStr));
                        else
                        {
                            byte[] existingKeyBin = existingKeyValue as byte[];
                            if (existingKeyBin == null)
                            {
                                EventLog.WriteEntry(string.Format("Device {0} key {1} invalid, replacing.", deviceMacAddress, keyStr));
                            }
                            else
                            {
                                if (!existingKeyBin.SequenceEqual(keyBin))
                                    EventLog.WriteEntry(string.Format("Device {0} key {1} different, replacing.", deviceMacAddress, keyStr));
                                else
                                {
                                    continue;
                                }
                            }
                        }

                        deviceRegKey.SetValue(keyStr, keyBin);
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            // 
            // BtKeySync
            // 
            this.ServiceName = "BtKeySync";

        }
    }
}
