using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MissionPlanner.MyCode
{
    internal class ConnectHelper
    {


        public void decorateGui(string connetPath, Panel connectContainer, EventHandler connectClickHandler)
        {
            if (File.Exists(connetPath))
            {
                string[] lines = File.ReadAllLines(connetPath);
                Regex tcp = new Regex("(.+)=tcp://(.*):([0-9]+)");
                Regex udp = new Regex("(.+)=udp://(.*):([0-9]+)");
                Regex udpcl = new Regex("(.+)=udpcl://(.*):([0-9]+)");
                Regex serial = new Regex("(.+)=serial:(.*):([0-9]+)");


                connectContainer.Height = lines.Length * 28 + 30;

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    
                    
                    if (tcp.IsMatch(line))
                    {
                        Match matches = tcp.Match(line);                        
                        addConnectionToList(connectContainer, fillConnectionDataList("tcp", matches), i, connectClickHandler);

                    }
                    else if (udp.IsMatch(line))
                    {
                        var matches = udp.Match(line);
                        addConnectionToList(connectContainer, fillConnectionDataList("udp", matches), i, connectClickHandler);
                    }
                    else if (udpcl.IsMatch(line))
                    {
                        var matches = udpcl.Match(line);
                        addConnectionToList(connectContainer, fillConnectionDataList("udpcl", matches), i, connectClickHandler);
                    }
                    else if (serial.IsMatch(line))
                    {
                        var matches = serial.Match(line);
                        addConnectionToList(connectContainer, fillConnectionDataList("serial", matches), i, connectClickHandler);
                    }
                }
                
            }
        }

        private List<string> fillConnectionDataList(string connectionType, Match matches)
        {
            List<string> res = new List<string>();
            res.Add(connectionType);

            for (int imatchIndex = 1; imatchIndex < matches.Groups.Count; imatchIndex++)
            {
                res.Add(matches.Groups[imatchIndex].Value);
            }

            
            return res;
        }

        internal void connectToPlane(string connectType, string connectMetaData)
        {
            MAVLinkInterface mav = new MAVLinkInterface();
            try
            {
                MainV2.instance.doConnect(mav, connectType, connectMetaData);

                MainV2.Comports.Add(mav);

                MainV2._connectionControl.UpdateSysIDS();
            }
            catch (Exception)
            {
            }
        }


        private void addConnectionToList(Panel container, List<string> connectionData, int i, EventHandler connectHandler)
        {
            RadioButton connect = new RadioButton();
            connect.Font = new Font("Times New Roman", 14);
            connect.Text = connectionData[1];
            connect.ForeColor = Color.Black;
            connect.BackColor = Color.LightGray;
            connect.Location = new Point(1, (i * 28) + 29);
            connect.Click += connectHandler;           
            connect.Tag = connectionData;
            container.Controls.Add(connect);

        }
    }
}
