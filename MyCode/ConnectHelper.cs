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


        public void decorateGui(string connetPath, TableLayoutPanel connectContainer, EventHandler connectClickHandler)
        {
            if (File.Exists(connetPath))
            {
                string[] lines = File.ReadAllLines(connetPath);
                Regex tcp = new Regex("(.+)=tcp://(.*):([0-9]+)");
                Regex udp = new Regex("(.+)=udp://(.*):([0-9]+)");
                Regex udpcl = new Regex("(.+)=udpcl://(.*):([0-9]+)");
                Regex serial = new Regex("(.+)=serial:(.*):([0-9]+)");


                connectContainer.RowCount = lines.Length;
                connectContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));

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
                /*
                    (lines, line =>
                //foreach (var line in lines)
                {
                    try
                    {
                        MAVLinkInterface mav = new MAVLinkInterface();

                        if (tcp.IsMatch(line))
                        {
                            var matches = tcp.Match(line);
                            var tc = new TcpSerial();
                            tc.client = new TcpClient(matches.Groups[1].Value, int.Parse(matches.Groups[2].Value));
                            mav.BaseStream = tc;
                        }
                        else if (udp.IsMatch(line))
                        {
                            var matches = udp.Match(line);
                            var uc = new UdpSerial(new UdpClient(int.Parse(matches.Groups[2].Value)));
                            uc.Port = matches.Groups[2].Value;
                            mav.BaseStream = uc;
                        }
                        else if (udpcl.IsMatch(line))
                        {
                            var matches = udpcl.Match(line);
                            var udc = new UdpSerialConnect();
                            udc.Port = matches.Groups[2].Value;
                            udc.client = new UdpClient(matches.Groups[1].Value, int.Parse(matches.Groups[2].Value));
                            mav.BaseStream = udc;
                        }
                        else if (serial.IsMatch(line))
                        {
                            var matches = serial.Match(line);
                            var port = new Comms.SerialPort();
                            port.PortName = matches.Groups[1].Value;
                            port.BaudRate = int.Parse(matches.Groups[2].Value);
                            mav.BaseStream = port;
                            mav.BaseStream.Open();
                        }
                        else
                        {
                            return;
                        }

                */



                // TableLayoutPanel Initialization
                /*
                panel.ColumnCount = 3;
        panel.RowCount = 1;
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        panel.Controls.Add(new Label() { Text = "Address" }, 1, 0);
        panel.Controls.Add(new Label() { Text = "Contact No" }, 2, 0);
        panel.Controls.Add(new Label() { Text = "Email ID" }, 3, 0);

        // For Add New Row (Loop this code for add multiple rows)
        panel.RowCount = panel.RowCount + 1;
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        panel.Controls.Add(new Label() { Text = "Street, City, State" }, 1, panel.RowCount - 1);
        panel.Controls.Add(new Label() { Text = "888888888888" }, 2, panel.RowCount - 1);
        panel.Controls.Add(new Label()

        */


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


        private void addConnectionToList(TableLayoutPanel container, List<string> connectionData, int i, EventHandler connectHandler)
        {
            RadioButton connect = new RadioButton();
            connect.Font = new Font("Times New Roman", 14);
            connect.Dock = DockStyle.Fill;
            connect.Text = connectionData[1];
            
            connect.Click += connectHandler;           
            connect.Tag = connectionData;
            container.Controls.Add(connect, 0, i);

        }
    }
}
