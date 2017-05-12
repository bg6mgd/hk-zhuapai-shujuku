using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Threading;

namespace AlarmCSharpDemo
{
    public partial class AlarmDemo : Form
    {
        private string sid;
        private string sctlj;
        private string scplj;
        private string scphm;
        private Int32 m_lRealHandle = -1;
        private CHCNetSDK.REALDATACALLBACK m_fRealData = null;
        private IntPtr m_ptrRealHandle;
        private Int32 m_lUserID = -1;
        private Int32[] m_lAlarmHandle = new Int32[200];
        private Int32 iListenHandle = -1; 
        private int iDeviceNumber = 0; //添加设备个数
        private uint iLastErr = 0;
        private string strErr;
        private delegate void MyDel();
        private CHCNetSDK.MSGCallBack m_falarmData = null;
        public delegate void UpdateListBoxCallback(string strAlarmTime, string strDevIP, string strAlarmMsg);

        CHCNetSDK.NET_VCA_TRAVERSE_PLANE m_struTraversePlane = new CHCNetSDK.NET_VCA_TRAVERSE_PLANE();
        CHCNetSDK.NET_VCA_AREA m_struVcaArea = new CHCNetSDK.NET_VCA_AREA();
        CHCNetSDK.NET_VCA_INTRUSION m_struIntrusion = new CHCNetSDK.NET_VCA_INTRUSION();
        CHCNetSDK.UNION_STATFRAME m_struStatFrame = new CHCNetSDK.UNION_STATFRAME();
        CHCNetSDK.UNION_STATTIME m_struStatTime = new CHCNetSDK.UNION_STATTIME();
        public AlarmDemo()
        {
            InitializeComponent();
            LoadData();
            //dgvClass.Dock = System.Windows.Forms.DockStyle.Fill;
            dgvClass.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //dgvClass.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            CheckForIllegalCrossThreadCalls = false;
            //ConsoleEx.AllocConsole();//打开控制台
            bool m_bInitSDK = CHCNetSDK.NET_DVR_Init();
            if (m_bInitSDK == false)
            {
                MessageBox.Show("NET_DVR_Init error!");
                return;
            }
            else
            {
                byte[] strIP = new byte[16 * 16];
                uint dwValidNum=0;
                Boolean bEnableBind=false;

                //获取本地PC网卡IP信息
                if (CHCNetSDK.NET_DVR_GetLocalIP(strIP, ref dwValidNum, ref bEnableBind))
                {
                    if (dwValidNum > 0)
                    {
                        //取第一张网卡的IP地址为默认监听端口
                        textBoxListenIP.Text = System.Text.Encoding.UTF8.GetString(strIP, 0, 16);
                    }
                
                }

                //保存SDK日志 To save the SDK log
                CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
                for (int i = 0; i < 20; i++)
                {
                    m_lAlarmHandle[i] = -1;
                }

                //设置报警回调函数
  
                m_falarmData = new CHCNetSDK.MSGCallBack(MsgCallback);
                CHCNetSDK.NET_DVR_SetDVRMessageCallBack_V30(m_falarmData, IntPtr.Zero);

            }
        }
        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            //    MyDebugInfo AlarmInfo = new MyDebugInfo(DebugInfo);
            //    switch (dwDataType)
            //    {
            //        case CHCNetSDK.NET_DVR_SYSHEAD:     // sys head
            //            if (!PlayCtrl.PlayM4_GetPort(ref m_lPort))
            //            {
            //                MessageBox.Show("Get Port Fail");
            //            }

            //            if (dwBufSize > 0)
            //              {
            //                //set as stream mode, real-time stream under preview
            //                if (!PlayCtrl.PlayM4_SetStreamOpenMode(m_lPort, PlayCtrl.STREAME_REALTIME))
            //                {
            //                    this.BeginInvoke(AlarmInfo, "PlayM4_SetStreamOpenMode fail");
            //                }
            //                //start player
            //                if (!PlayCtrl.PlayM4_OpenStream(m_lPort, ref pBuffer, dwBufSize, 1024 * 1024))
            //                {
            //                    m_lPort = -1;
            //                    this.BeginInvoke(AlarmInfo, "PlayM4_OpenStream fail");
            //                    break;
            //                }
            //set soft decode display callback function to capture
            //                m_fDisplayFun = new PlayCtrl.DISPLAYCBFUN(RemoteDisplayCBFun);
            //                if (!PlayCtrl.PlayM4_SetDisplayCallBack(m_lPort, m_fDisplayFun))
            //                {
            //                    this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayCallBack fail");
            //                }

            //start play, set play window
            //                this.BeginInvoke(AlarmInfo, "About to call PlayM4_Play");

            //                if (!PlayCtrl.PlayM4_Play(m_lPort, m_ptrRealHandle))
            //                {
            //                    m_lPort = -1;
            //                    this.BeginInvoke(AlarmInfo, "PlayM4_Play fail");
            //                    break;
            //                }

            //set frame buffer number

            //                if (!PlayCtrl.PlayM4_SetDisplayBuf(m_lPort, 15))
            //                {
            //                    this.BeginInvoke(AlarmInfo, "PlayM4_SetDisplayBuf fail");
            //                }

            //set display mode
            //                if (!PlayCtrl.PlayM4_SetOverlayMode(m_lPort, 0, 0/* COLORREF(0)*/))//play off screen // todo!!!
            //                {
            //                    this.BeginInvoke(AlarmInfo, " PlayM4_SetOverlayMode fail");
            //                }
            //            }

            //            break;
            //        case CHCNetSDK.NET_DVR_STREAMDATA:     // video stream data
            //            if (dwBufSize > 0 && m_lPort != -1)
            //            {
            //                if (!PlayCtrl.PlayM4_InputData(m_lPort, ref pBuffer, dwBufSize))
            //                {
            //                    this.BeginInvoke(AlarmInfo, " PlayM4_InputData fail");
            //                }
            //            }
            //            break;

            //        case CHCNetSDK.NET_DVR_AUDIOSTREAMDATA:     //  Audio Stream Data
            //            if (dwBufSize > 0 && m_lPort != -1)
            //            {
            //                if (!PlayCtrl.PlayM4_InputVideoData(m_lPort, ref pBuffer, dwBufSize))
            //                {
            //                   this.BeginInvoke(AlarmInfo, "PlayM4_InputVideoData Fail");
            //                }
            //            }

            //            break;
            //        default:
            //            break;
            //    } */

        }
   

        public void MsgCallback(int lCommand, ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
            switch (lCommand)
            {
                case CHCNetSDK.COMM_ALARM: //(DS-8000老设备)移动侦测、视频丢失、遮挡、IO信号量等报警信息
                    ProcessCommAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ALARM_V30://移动侦测、视频丢失、遮挡、IO信号量等报警信息
                    ProcessCommAlarm_V30(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ALARM_RULE://进出区域、入侵、徘徊、人员聚集等行为分析报警信息
                    ProcessCommAlarm_RULE(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_UPLOAD_PLATE_RESULT://交通抓拍结果上传(老报警信息类型)
                    ProcessCommAlarm_Plate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ITS_PLATE_RESULT://交通抓拍结果上传(新报警信息类型)
                    ProcessCommAlarm_ITSPlate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ALARM_PDC://客流量统计报警信息
                    ProcessCommAlarm_PDC(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ITS_PARK_VEHICLE://客流量统计报警信息
                    ProcessCommAlarm_PARK(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_DIAGNOSIS_UPLOAD://VQD报警信息
                    ProcessCommAlarm_VQD(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_UPLOAD_FACESNAP_RESULT://人脸抓拍结果信息
                    ProcessCommAlarm_FaceSnap(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ALARM_FACE_DETECTION://人脸侦测报警信息
                    ProcessCommAlarm_FaceDetect(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                case CHCNetSDK.COMM_ALARMHOST_CID_ALARM://报警主机CID报警上传
                    ProcessCommAlarm_CIDAlarm(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                    break;
                default:
                    break;
            }
        }

        public void ProcessCommAlarm(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_ALARMINFO struAlarmInfo = new CHCNetSDK.NET_DVR_ALARMINFO();

            struAlarmInfo = (CHCNetSDK.NET_DVR_ALARMINFO)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_ALARMINFO));

            string strIP = pAlarmer.sDeviceIP;
            string stringAlarm = "";
            int i = 0;

            switch (struAlarmInfo.dwAlarmType)
            {
                case 0:
                    stringAlarm = "信号量报警，报警报警输入口：" + struAlarmInfo.dwAlarmInputNumber + "，触发录像通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    {
                        if (struAlarmInfo.dwAlarmRelateChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 1:
                    stringAlarm = "硬盘满，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                    {
                        if (struAlarmInfo.dwDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 2:
                    stringAlarm = "信号丢失，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    {
                        if (struAlarmInfo.dwChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 3:
                    stringAlarm = "移动侦测，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    {
                        if (struAlarmInfo.dwChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 4:
                    stringAlarm = "硬盘未格式化，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                    {
                        if (struAlarmInfo.dwDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 5:
                    stringAlarm = "读写硬盘出错，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM; i++)
                    {
                        if (struAlarmInfo.dwDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 6:
                    stringAlarm = "遮挡报警，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    {
                        if (struAlarmInfo.dwChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 7:
                    stringAlarm = "制式不匹配，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM; i++)
                    {
                        if (struAlarmInfo.dwChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 8:
                    stringAlarm = "非法访问";
                    break;
                default:
                    stringAlarm = "其他未知报警信息";
                    break;
            }

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString();
                paras[1] = strIP;
                paras[2] = stringAlarm;                
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(),strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_V30(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {

            CHCNetSDK.NET_DVR_ALARMINFO_V30 struAlarmInfoV30 = new CHCNetSDK.NET_DVR_ALARMINFO_V30();

            struAlarmInfoV30 = (CHCNetSDK.NET_DVR_ALARMINFO_V30)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_ALARMINFO_V30));

            string strIP = pAlarmer.sDeviceIP;
            string stringAlarm = "";
            int i;

            switch (struAlarmInfoV30.dwAlarmType)
            {
                case 0:
                    stringAlarm = "信号量报警，报警报警输入口：" + struAlarmInfoV30.dwAlarmInputNumber + "，触发录像通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byAlarmRelateChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + "\\";
                        }
                    }
                    break;
                case 1:
                    stringAlarm = "硬盘满，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " ";
                        }
                    }
                    break;
                case 2:
                    stringAlarm = "信号丢失，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 3:
                    stringAlarm = "移动侦测，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 4:
                    stringAlarm = "硬盘未格式化，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 5:
                    stringAlarm = "读写硬盘出错，报警硬盘号：";
                    for (i = 0; i < CHCNetSDK.MAX_DISKNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byDiskNumber[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 6:
                    stringAlarm = "遮挡报警，报警通道：";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 7:
                    stringAlarm = "制式不匹配，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 8:
                    stringAlarm = "非法访问";
                    break;
                case 9:
                    stringAlarm = "视频信号异常，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 10:
                    stringAlarm = "录像/抓图异常，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 11:
                    stringAlarm = "智能场景变化，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 12:
                    stringAlarm = "阵列异常";
                    break;
                case 13:
                    stringAlarm = "前端/录像分辨率不匹配，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                case 15:
                    stringAlarm = "智能侦测，报警通道";
                    for (i = 0; i < CHCNetSDK.MAX_CHANNUM_V30; i++)
                    {
                        if (struAlarmInfoV30.byChannel[i] == 1)
                        {
                            stringAlarm += (i + 1) + " \\ ";
                        }
                    }
                    break;
                default:
                    stringAlarm = "其他未知报警信息";
                    break;
            }

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString();
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }

        }

        private void ProcessCommAlarm_RULE(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_VCA_RULE_ALARM struRuleAlarmInfo = new CHCNetSDK.NET_VCA_RULE_ALARM();
            struRuleAlarmInfo = (CHCNetSDK.NET_VCA_RULE_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_RULE_ALARM));

            //报警信息
            string stringAlarm = "";
            uint dwSize = (uint)Marshal.SizeOf(struRuleAlarmInfo.struRuleInfo.uEventParam);

            switch (struRuleAlarmInfo.struRuleInfo.wEventTypeEx)
            {
                case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_TRAVERSE_PLANE:
                    IntPtr ptrTraverseInfo = Marshal.AllocHGlobal((Int32)dwSize);
                    Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrTraverseInfo, false);
                    m_struTraversePlane = (CHCNetSDK.NET_VCA_TRAVERSE_PLANE)Marshal.PtrToStructure(ptrTraverseInfo, typeof(CHCNetSDK.NET_VCA_TRAVERSE_PLANE));
                    stringAlarm = "穿越警戒面，目标ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
                    //警戒面边线起点坐标: (m_struTraversePlane.struPlaneBottom.struStart.fX, m_struTraversePlane.struPlaneBottom.struStart.fY)
                    //警戒面边线终点坐标: (m_struTraversePlane.struPlaneBottom.struEnd.fX, m_struTraversePlane.struPlaneBottom.struEnd.fY)
                    break;
                case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_ENTER_AREA:
                    IntPtr ptrEnterInfo = Marshal.AllocHGlobal((Int32)dwSize);
                    Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrEnterInfo, false);
                    m_struVcaArea = (CHCNetSDK.NET_VCA_AREA)Marshal.PtrToStructure(ptrEnterInfo, typeof(CHCNetSDK.NET_VCA_AREA));
                    stringAlarm = "目标进入区域，目标ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
                    //m_struVcaArea.struRegion 多边形区域坐标
                    break;
                case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_EXIT_AREA:               
                    IntPtr ptrExitInfo = Marshal.AllocHGlobal((Int32)dwSize);
                    Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrExitInfo, false);
                    m_struVcaArea = (CHCNetSDK.NET_VCA_AREA)Marshal.PtrToStructure(ptrExitInfo, typeof(CHCNetSDK.NET_VCA_AREA));
                    stringAlarm = "目标离开区域，目标ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
                    //m_struVcaArea.struRegion 多边形区域坐标
                    break;
                case (ushort)CHCNetSDK.VCA_RULE_EVENT_TYPE_EX.ENUM_VCA_EVENT_INTRUSION:
                    IntPtr ptrIntrusionInfo = Marshal.AllocHGlobal((Int32)dwSize);
                    Marshal.StructureToPtr(struRuleAlarmInfo.struRuleInfo.uEventParam, ptrIntrusionInfo, false);
                    m_struIntrusion = (CHCNetSDK.NET_VCA_INTRUSION)Marshal.PtrToStructure(ptrIntrusionInfo, typeof(CHCNetSDK.NET_VCA_INTRUSION));

                    int i = 0;
                    string strRegion = "";
                    for (i = 0; i < m_struIntrusion.struRegion.dwPointNum; i++)
                    {
                        strRegion = strRegion + "(" + m_struIntrusion.struRegion.struPos[i].fX + "," + m_struIntrusion.struRegion.struPos[i].fY + ")";
                    }
                    stringAlarm = "周界入侵，目标ID：" + struRuleAlarmInfo.struTargetInfo.dwID + "，区域范围：" + strRegion;
                    //m_struIntrusion.struRegion 多边形区域坐标
                    break;
                default:
                    stringAlarm = "其他行为分析报警，目标ID：" + struRuleAlarmInfo.struTargetInfo.dwID;
                    break;            
            }


            //报警图片保存
            if (struRuleAlarmInfo.dwPicDataLen > 0)
            {
                FileStream fs = new FileStream("行为分析报警抓图.jpg", FileMode.Create);
                int iLen = (int)struRuleAlarmInfo.dwPicDataLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struRuleAlarmInfo.pImage, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }

            //报警时间：年月日时分秒
            string strTimeYear = ((struRuleAlarmInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struRuleAlarmInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struRuleAlarmInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struRuleAlarmInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struRuleAlarmInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struRuleAlarmInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;
           
            //报警设备IP地址
            string strIP = struRuleAlarmInfo.struDevInfo.struDevIP.sIpV4;

            //将报警信息添加进列表
            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = strTime;
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(strTime, strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_Plate(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_PLATE_RESULT struPlateResultInfo = new CHCNetSDK.NET_DVR_PLATE_RESULT();
            uint dwSize = (uint)Marshal.SizeOf(struPlateResultInfo);

            struPlateResultInfo = (CHCNetSDK.NET_DVR_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_PLATE_RESULT));

            //保存抓拍图片
            string str = "";
            if (struPlateResultInfo.byResultType == 1 && struPlateResultInfo.dwPicLen != 0)
            {
                str = "D:/UserID_" + pAlarmer.lUserID + "_近景图.jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struPlateResultInfo.dwPicLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struPlateResultInfo.pBuffer1, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }
            if (struPlateResultInfo.dwPicPlateLen != 0)
            {
                str = "D:/UserID_" + pAlarmer.lUserID + "_车牌图.jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struPlateResultInfo.dwPicPlateLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struPlateResultInfo.pBuffer2, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }
            if (struPlateResultInfo.dwFarCarPicLen != 0)
            {
                str = "D:/UserID_" + pAlarmer.lUserID + "_远景图.jpg";
                FileStream fs = new FileStream(str, FileMode.Create);
                int iLen = (int)struPlateResultInfo.dwFarCarPicLen;
                byte[] by = new byte[iLen];
                Marshal.Copy(struPlateResultInfo.pBuffer5, by, 0, iLen);
                fs.Write(by, 0, iLen);
                fs.Close();
            }

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //抓拍时间：年月日时分秒
            string strTimeYear = System.Text.Encoding.UTF8.GetString(struPlateResultInfo.byAbsTime);

            //上传结果
            string stringAlarm = "抓拍上传，" + "车牌：" + struPlateResultInfo.struPlateInfo.sLicense + "，车辆序号：" + struPlateResultInfo.struVehicleInfo.dwIndex; 

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = strTimeYear; //当前PC系统时间为DateTime.Now.ToString();
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }
        private void LoadData()
        {
            List<clxx> list = new List<clxx>();
            string constr = "Data Source=(local);Initial Catalog=ww;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(constr))
            {
                //string sql = "select * from cp";
                string sql ="select* from cp order  by ID  desc";
               
                    using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //判断是否查询到了数据
                        if (reader.HasRows)
                        {
                            //一条一条读取数据
                            while (reader.Read())
                            {
                                //tClassId, tClassName, tClassDesc
                                clxx model = new clxx();
                                model.clID = reader.GetInt32(0);
                                model.chetlj = reader.GetString(1);
                                model.cheplj = reader.IsDBNull(2) ? null : reader.GetString(2);
                                model.chephm = reader.IsDBNull(3) ? null : reader.GetString(3);
                                list.Add(model);//把model对象加到list集合中
                               
                            }
                        }
                    }
                }
                
            }

            //数据绑定需要注意的一点：
            //数据绑定的时候，只认“属性”，不认“字段”。内部通过反射来实现。
            this.dgvClass.DataSource = list;//数据绑定
            //dgvClass.FirstDisplayedScrollingRowIndex = dgvClass.RowCount - 1;
            
        }
        private void ProcessCommAlarm_ITSPlate(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_ITS_PLATE_RESULT struITSPlateResult = new CHCNetSDK.NET_ITS_PLATE_RESULT();
            uint dwSize = (uint)Marshal.SizeOf(struITSPlateResult);
            clxx CLxx = new clxx();

            struITSPlateResult = (CHCNetSDK.NET_ITS_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_ITS_PLATE_RESULT));

            //保存抓拍图片
            for (int i = 0; i < struITSPlateResult.dwPicNum; i++)
            {
                if (struITSPlateResult.struPicInfo[i].dwDataLen != 0)
                {
                   
                    string str1 = string.Format("{0:D4}", struITSPlateResult.struSnapFirstPicTime.wYear) +
                string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byMonth) +
                string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byDay) + ""
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byHour) + ":"
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byMinute) + ":"
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.bySecond) + ":"
                + string.Format("{0:D3}", struITSPlateResult.struSnapFirstPicTime.wMilliSec);
                    str1 = str1.Replace("-", "").Replace(":", "").Replace(" ", "");
                   // Console.Write(str1);

                    string str="";
                    switch (struITSPlateResult.struPicInfo[i].byType)
                    {
                        case 0:
                            //车牌图片
                           
                            str = "D:\\1\\"+str1 +"1.jpg";
                            CLxx.cheplj = str;
                            break;
                        case 1:
                            //车头图片
                      
                            str = "D:\\1\\"+ str1 +"0.jpg";
                            CLxx.chetlj = str;
                            break;
                           
                        default:
                            break;
                    }
                    FileStream fs = new FileStream(str, FileMode.Create);
                    int iLen = (int)struITSPlateResult.struPicInfo[i].dwDataLen;
                    byte[] by = new byte[iLen];
                    Marshal.Copy(struITSPlateResult.struPicInfo[i].pBuffer, by, 0, iLen);
                    fs.Write(by, 0, iLen);
                    fs.Close();
                    if (str.Substring(17, 1)=="1")
                    {
                        pic3.Image = Image.FromFile(str);
                       
                        
                    }
                    else 
                    {
                        pic2.Image = Image.FromFile(str);
                        
                    }

                    //Thread thread = new Thread(new ThreadStart(LoadData));
                    //thread.Start();
                    // CLxx = null;

                }

               
            }
            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //抓拍时间：年月日时分秒
            string strTimeYear = string.Format("{0:D4}", struITSPlateResult.struSnapFirstPicTime.wYear) + 
                string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byMonth) + 
                string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byDay) + " " 
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byHour) + ":" 
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.byMinute) + ":" 
                + string.Format("{0:D2}", struITSPlateResult.struSnapFirstPicTime.bySecond) + ":" 
                + string.Format("{0:D3}", struITSPlateResult.struSnapFirstPicTime.wMilliSec);
            strTimeYear = strTimeYear.Replace("-", "").Replace(":", "");
            //上传结果
            string stringAlarm = "抓拍上传，" + "车牌：" + struITSPlateResult.struPlateInfo.sLicense + "，车辆序号：" + struITSPlateResult.struVehicleInfo.dwIndex;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = strTimeYear;//当前系统时间为：DateTime.Now.ToString();
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
            string constr = "Data Source=(local);Initial Catalog=ww;Integrated Security=True";
            using (SqlConnection con = new SqlConnection(constr))
            {
                CLxx.chephm = struITSPlateResult.struPlateInfo.sLicense;
                string sql = string.Format("insert into cp output inserted.ID values(N'{0}',N'{1}',N'{2}')", CLxx.chetlj, CLxx.cheplj, CLxx.chephm);
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    con.Open();
                    object obj = cmd.ExecuteScalar();


                }
            }
            //LoadData();
            //Thread test = new Thread(new ThreadStart(LoadData));// 重新加载，绑定DataGridView
            // test.Start();
            this.Invoke(new MethodInvoker(LoadData));
            //dgvClass.ClearSelection();
        }

        private void ProcessCommAlarm_PDC(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_PDC_ALRAM_INFO struPDCInfo = new CHCNetSDK.NET_DVR_PDC_ALRAM_INFO();
            uint dwSize = (uint)Marshal.SizeOf(struPDCInfo);
            struPDCInfo = (CHCNetSDK.NET_DVR_PDC_ALRAM_INFO)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_PDC_ALRAM_INFO));

            string stringAlarm = "客流量统计，进入人数：" + struPDCInfo.dwEnterNum + "，离开人数：" + struPDCInfo.dwLeaveNum;

            uint dwUnionSize = (uint)Marshal.SizeOf(struPDCInfo.uStatModeParam);
            IntPtr ptrPDCUnion = Marshal.AllocHGlobal((Int32)dwUnionSize);
            Marshal.StructureToPtr(struPDCInfo.uStatModeParam, ptrPDCUnion, false);

            if (struPDCInfo.byMode == 0) //单帧统计结果，此处为UTC时间
            {              
                m_struStatFrame = (CHCNetSDK.UNION_STATFRAME)Marshal.PtrToStructure(ptrPDCUnion, typeof(CHCNetSDK.UNION_STATFRAME));       
                stringAlarm = stringAlarm + "，单帧统计，相对时标：" + m_struStatFrame.dwRelativeTime + "，绝对时标：" + m_struStatFrame.dwAbsTime;
            }
            if (struPDCInfo.byMode == 1) //最小时间段统计结果
            {
                m_struStatTime = (CHCNetSDK.UNION_STATTIME)Marshal.PtrToStructure(ptrPDCUnion, typeof(CHCNetSDK.UNION_STATTIME));

                //开始时间
                string strStartTime = string.Format("{0:D4}", m_struStatTime.tmStart.dwYear) +
                string.Format("{0:D2}", m_struStatTime.tmStart.dwMonth) +
                string.Format("{0:D2}", m_struStatTime.tmStart.dwDay) + " "
                + string.Format("{0:D2}", m_struStatTime.tmStart.dwHour) + ":"
                + string.Format("{0:D2}", m_struStatTime.tmStart.dwMinute) + ":"
                + string.Format("{0:D2}", m_struStatTime.tmStart.dwSecond);

                //结束时间
                string strEndTime = string.Format("{0:D4}", m_struStatTime.tmEnd.dwYear) +
                string.Format("{0:D2}", m_struStatTime.tmEnd.dwMonth) +
                string.Format("{0:D2}", m_struStatTime.tmEnd.dwDay) + " "
                + string.Format("{0:D2}", m_struStatTime.tmEnd.dwHour) + ":"
                + string.Format("{0:D2}", m_struStatTime.tmEnd.dwMinute) + ":"
                + string.Format("{0:D2}", m_struStatTime.tmEnd.dwSecond);

                stringAlarm = stringAlarm + "，最小时间段统计，开始时间：" + strStartTime + "，结束时间：" + strEndTime;
            }
            Marshal.FreeHGlobal(ptrPDCUnion);

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;


            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }


        private void ProcessCommAlarm_PARK(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_ITS_PARK_VEHICLE struParkInfo = new CHCNetSDK.NET_ITS_PARK_VEHICLE();
            uint dwSize = (uint)Marshal.SizeOf(struParkInfo);
            struParkInfo = (CHCNetSDK.NET_ITS_PARK_VEHICLE)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_ITS_PARK_VEHICLE));

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //保存抓拍图片
            for (int i = 0; i < struParkInfo.dwPicNum; i++)
            {
                if ((struParkInfo.struPicInfo[i].dwDataLen != 0) && (struParkInfo.struPicInfo[i].pBuffer != IntPtr.Zero))
                {
                    string str = "D:/Device_[" + strIP + "]_Pictype_" + struParkInfo.struPicInfo[i].byType + "_Num" + (i + 1) + ".jpg";
                    FileStream fs = new FileStream(str, FileMode.Create);
                    int iLen = (int)struParkInfo.struPicInfo[i].dwDataLen;
                    byte[] by = new byte[iLen];
                    Marshal.Copy(struParkInfo.struPicInfo[i].pBuffer, by, 0, iLen);
                    fs.Write(by, 0, iLen);
                    fs.Close();
                }
            }

            string stringAlarm = "停车场数据上传，异常状态：" + struParkInfo.byParkError + "，车位编号：" + struParkInfo.byParkingNo +
                ", 车辆状态：" + struParkInfo.byLocationStatus + "，车牌号码：" + struParkInfo.struPlateInfo.sLicense;           

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_VQD(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_DIAGNOSIS_UPLOAD struVQDInfo = new CHCNetSDK.NET_DVR_DIAGNOSIS_UPLOAD();
            uint dwSize = (uint)Marshal.SizeOf(struVQDInfo);
            struVQDInfo = (CHCNetSDK.NET_DVR_DIAGNOSIS_UPLOAD)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_DIAGNOSIS_UPLOAD));

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //开始时间
            string strCheckTime = string.Format("{0:D4}", struVQDInfo.struCheckTime.dwYear) +
            string.Format("{0:D2}", struVQDInfo.struCheckTime.dwMonth) +
            string.Format("{0:D2}", struVQDInfo.struCheckTime.dwDay) + " "
            + string.Format("{0:D2}", struVQDInfo.struCheckTime.dwHour) + ":"
            + string.Format("{0:D2}", struVQDInfo.struCheckTime.dwMinute) + ":"
            + string.Format("{0:D2}", struVQDInfo.struCheckTime.dwSecond);

            string stringAlarm = "视频质量诊断结果，流ID：" + struVQDInfo.sStreamID + "，监测点IP：" + struVQDInfo.sMonitorIP + "，监控点通道号：" + struVQDInfo.dwChanIndex +
                "，检测时间：" + strCheckTime + "，byResult：" + struVQDInfo.byResult + "，bySignalResult：" + struVQDInfo.bySignalResult + "，byBlurResult：" + struVQDInfo.byBlurResult;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_FaceSnap(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_VCA_FACESNAP_RESULT struFaceSnapInfo = new CHCNetSDK.NET_VCA_FACESNAP_RESULT();
            uint dwSize = (uint)Marshal.SizeOf(struFaceSnapInfo);
            struFaceSnapInfo = (CHCNetSDK.NET_VCA_FACESNAP_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_VCA_FACESNAP_RESULT));

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //报警时间：年月日时分秒
            string strTimeYear = ((struFaceSnapInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struFaceSnapInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struFaceSnapInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struFaceSnapInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struFaceSnapInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struFaceSnapInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

            string stringAlarm = "人脸抓拍结果结果，前端设备：" + struFaceSnapInfo.struDevInfo.struDevIP + "，报警时间：" + strTime;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_FaceDetect(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_FACE_DETECTION struFaceDetectInfo = new CHCNetSDK.NET_DVR_FACE_DETECTION();
            uint dwSize = (uint)Marshal.SizeOf(struFaceDetectInfo);
            struFaceDetectInfo = (CHCNetSDK.NET_DVR_FACE_DETECTION)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_FACE_DETECTION));

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //报警时间：年月日时分秒
            string strTimeYear = ((struFaceDetectInfo.dwAbsTime >> 26) + 2000).ToString();
            string strTimeMonth = ((struFaceDetectInfo.dwAbsTime >> 22) & 15).ToString("d2");
            string strTimeDay = ((struFaceDetectInfo.dwAbsTime >> 17) & 31).ToString("d2");
            string strTimeHour = ((struFaceDetectInfo.dwAbsTime >> 12) & 31).ToString("d2");
            string strTimeMinute = ((struFaceDetectInfo.dwAbsTime >> 6) & 63).ToString("d2");
            string strTimeSecond = ((struFaceDetectInfo.dwAbsTime >> 0) & 63).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

            string stringAlarm = "人脸抓拍结果结果，前端设备：" + struFaceDetectInfo.struDevInfo.struDevIP + "，报警时间：" + strTime;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }

        private void ProcessCommAlarm_CIDAlarm(ref CHCNetSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
        {
            CHCNetSDK.NET_DVR_CID_ALARM struCIDAlarm = new CHCNetSDK.NET_DVR_CID_ALARM();
            uint dwSize = (uint)Marshal.SizeOf(struCIDAlarm);
            struCIDAlarm = (CHCNetSDK.NET_DVR_CID_ALARM)Marshal.PtrToStructure(pAlarmInfo, typeof(CHCNetSDK.NET_DVR_CID_ALARM));

            //报警设备IP地址
            string strIP = pAlarmer.sDeviceIP;

            //报警时间：年月日时分秒
            string strTimeYear = (struCIDAlarm.struTriggerTime.wYear).ToString();
            string strTimeMonth = (struCIDAlarm.struTriggerTime.byMonth).ToString("d2");
            string strTimeDay = (struCIDAlarm.struTriggerTime.byDay).ToString("d2");
            string strTimeHour = (struCIDAlarm.struTriggerTime.byHour).ToString("d2");
            string strTimeMinute = (struCIDAlarm.struTriggerTime.byMinute).ToString("d2");
            string strTimeSecond = (struCIDAlarm.struTriggerTime.bySecond).ToString("d2");
            string strTime = strTimeYear + "-" + strTimeMonth + "-" + strTimeDay + " " + strTimeHour + ":" + strTimeMinute + ":" + strTimeSecond;

            string stringAlarm = "报警主机CID报告，sCIDCode：" + struCIDAlarm.sCIDCode + "，sCIDDescribe：" + struCIDAlarm.sCIDDescribe
                + "，报告类型：" + struCIDAlarm.byReportType + "，防区号：" + struCIDAlarm.wDefenceNo + "，报警触发时间：" + strTime;

            if (InvokeRequired)
            {
                object[] paras = new object[3];
                paras[0] = DateTime.Now.ToString(); //当前PC系统时间
                paras[1] = strIP;
                paras[2] = stringAlarm;
                listViewAlarmInfo.BeginInvoke(new UpdateListBoxCallback(UpdateClientList), paras);
            }
            else
            {
                //创建该控件的主线程直接更新信息列表 
                UpdateClientList(DateTime.Now.ToString(), strIP, stringAlarm);
            }
        }

        public void UpdateClientList(string strAlarmTime, string strDevIP, string strAlarmMsg)
        {
            //列表新增报警信息
            listViewAlarmInfo.Items.Add(new ListViewItem(new string[] { strAlarmTime, strDevIP, strAlarmMsg }));
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (textBoxIP.Text == "" || textBoxPort.Text == "" ||
                textBoxUserName.Text == "" || textBoxPassword.Text == "")
            {
                MessageBox.Show("Please input IP, Port, User name and Password!");
                return;
            }

            if (iDeviceNumber >= 20)
            {
                MessageBox.Show("本程序限制最多添加20台设备！");
                return;
            }

            string DVRIPAddress = textBoxIP.Text; //设备IP地址或者域名 Device IP
            Int16 DVRPortNumber = Int16.Parse(textBoxPort.Text);//设备服务端口号 Device Port
            string DVRUserName = textBoxUserName.Text;//设备登录用户名 User name to login
            string DVRPassword = textBoxPassword.Text;//设备登录密码 Password to login
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                strErr = "NET_DVR_Login_V30 failed, error code= " + iLastErr; //登录失败，输出错误号 Failed to login and output the error code
                MessageBox.Show(strErr);
                return;
            }
            else
            {
                //登录成功
                iDeviceNumber++;
                string str1 = "" + m_lUserID;
                listViewDevice.Items.Add(new ListViewItem(new string[] { str1, DVRIPAddress, "未布防" }));//将已注册设备添加进列表
            }
        }

        private void btn_SetAlarm_Click(object sender, EventArgs e)
        {
             CHCNetSDK.NET_DVR_SETUPALARM_PARAM struAlarmParam = new CHCNetSDK.NET_DVR_SETUPALARM_PARAM();
             struAlarmParam.dwSize = (uint)Marshal.SizeOf(struAlarmParam);
             struAlarmParam.byLevel = 1; //0- 一级布防,1- 二级布防
             struAlarmParam.byAlarmInfoType = 1;//智能交通设备有效，新报警信息类型
             struAlarmParam.byFaceAlarmDetection = 1;//1-人脸侦测

             for (int i = 0; i < iDeviceNumber; i++)
             {
                 m_lUserID = Int32.Parse(listViewDevice.Items[i].SubItems[0].Text);
                 m_lAlarmHandle[m_lUserID] = CHCNetSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID, ref struAlarmParam);
                 if (m_lAlarmHandle[m_lUserID] < 0)
                 {
                     iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                     strErr = "布防失败，错误号：" + iLastErr; //布防失败，输出错误号
                     listViewDevice.Items[i].SubItems[2].Text = strErr;              
                 }
                 else
                 {
                     listViewDevice.Items[i].SubItems[2].Text = "布防成功";                     
                 }
                 btn_SetAlarm.Enabled = false;
             }
         }

         private void btnCloseAlarm_Click(object sender, EventArgs e)
         {
             for (int i = 0; i < iDeviceNumber; i++)
             {
                 m_lUserID = Int32.Parse(listViewDevice.Items[i].SubItems[0].Text);
                 if (m_lAlarmHandle[m_lUserID] >= 0)
                 {
                     if (!CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle[m_lUserID]))
                     {
                         iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                         strErr = "撤防失败，错误号：" + iLastErr; //撤防失败，输出错误号
                         listViewDevice.Items[i].SubItems[2].Text = strErr;
                     }
                     else
                     {
                         listViewDevice.Items[i].SubItems[2].Text = "未布防";
                         m_lAlarmHandle[i] = -1;
                     }
                 }
                 else 
                 {
                     listViewDevice.Items[i].SubItems[2].Text = "未布防";                 
                 }
             }
             btn_SetAlarm.Enabled = true;
         }

         private void btnExit_Click(object sender, EventArgs e)
         {
             //撤防
             btnCloseAlarm_Click(sender,e);

             //停止监听
             if (iListenHandle >= 0)
             {
                 CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle);
             }

             //注销登录
             for (int i = 0; i < iDeviceNumber; i++)
             {
                 m_lUserID = Int32.Parse(listViewDevice.Items[i].SubItems[0].Text);
                 CHCNetSDK.NET_DVR_Logout(m_lUserID);                 
             }

             //释放SDK资源，在程序结束之前调用
             CHCNetSDK.NET_DVR_Cleanup();

             Application.Exit();
         }

         private void listViewDevice_MouseClick(object sender, MouseEventArgs e)
         {
             if (e.Button == MouseButtons.Right)
             {
                 if (listViewDevice.SelectedItems.Count > 0)
                 {
                     if (DialogResult.OK == MessageBox.Show("请确认是否删除所选择的设备！","删除提示",MessageBoxButtons.OKCancel))
                     {
                         foreach (ListViewItem item in this.listViewDevice.SelectedItems)
                         {
                             if (item.Selected)
                             {
                                 m_lUserID = Int32.Parse(item.SubItems[0].Text);
                                 CHCNetSDK.NET_DVR_CloseAlarmChan_V30(m_lAlarmHandle[m_lUserID]);
                                 CHCNetSDK.NET_DVR_Logout(m_lUserID);
                                 item.Remove();
                                 iDeviceNumber--;
                             }
                         }
                         this.listViewDevice.Refresh();
                     }                      
                 }
                 else
                 {
                     
                 }
             }
         }

         private void btnStartListen_Click(object sender, EventArgs e)
         {
             string sLocalIP = textBoxListenIP.Text;
             ushort wLocalPort = ushort.Parse(textBoxListenPort.Text);

             iListenHandle = CHCNetSDK.NET_DVR_StartListen_V30(sLocalIP, wLocalPort, m_falarmData, IntPtr.Zero);
             if (iListenHandle < 0)
             {
                 iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                 strErr = "启动监听失败，错误号：" + iLastErr; //撤防失败，输出错误号
                 MessageBox.Show(strErr);
             }
             else
             {
                 MessageBox.Show("成功启动监听！");
                 btnStopListen.Enabled = true;
                 btnStartListen.Enabled = false;
             }
         }

         private void btnStopListen_Click(object sender, EventArgs e)
         {
             if (!CHCNetSDK.NET_DVR_StopListen_V30(iListenHandle))
             {
                 iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                 strErr = "停止监听失败，错误号：" + iLastErr; //撤防失败，输出错误号
                 MessageBox.Show(strErr);
             }
             else
             {
                 MessageBox.Show("停止监听！");
                 btnStopListen.Enabled = false;
                 btnStartListen.Enabled = true;
             }
         }

        private void button1_Click(object sender, EventArgs e)
        {
            CHCNetSDK.NET_DVR_CLIENTINFO lpClientInfo = new CHCNetSDK.NET_DVR_CLIENTINFO();

            lpClientInfo.lChannel = 1;
            lpClientInfo.lLinkMode = 0x0000;
            lpClientInfo.sMultiCastIP = "";

            lpClientInfo.hPlayWnd = pic.Handle;
            m_ptrRealHandle = pic.Handle;
            m_fRealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);
            IntPtr pUser = new IntPtr();
            m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V30(m_lUserID, ref lpClientInfo, null/*m_fRealData*/, pUser, 1);


            if (m_lRealHandle == -1)
            {
                uint nError = CHCNetSDK.NET_DVR_GetLastError();
                // DebugInfo("NET_DVR_RealPlay fail %d!");
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CHCNetSDK.NET_DVR_SNAPCFG struSnapCfg = new CHCNetSDK.NET_DVR_SNAPCFG();
            struSnapCfg.wIntervalTime = new ushort[4];
            struSnapCfg.dwSize = (uint)Marshal.SizeOf(struSnapCfg);
            struSnapCfg.byRelatedDriveWay = 1;
            struSnapCfg.bySnapTimes = 1;
            struSnapCfg.wSnapWaitTime = 100;
            struSnapCfg.wIntervalTime[0] = 100;

            bool bManualSnap = CHCNetSDK.NET_DVR_ContinuousShoot(m_lUserID, ref struSnapCfg);

            if (!bManualSnap)
            {
                uint iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "NET_DVR_ContinuousShoot failed, error code= " + iLastErr;
                // DebugInfo(str);
                return;
            }
            
        }

        //private void listViewAlarmInfo_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (listViewAlarmInfo.SelectedItems.Count != 0)
        //    {
        //      string Text = listViewAlarmInfo.SelectedItems[0].Text;
        //        Text = Text.Replace(" ", "");
        //       // Console.WriteLine(Text);
        //        string str2 = "D:\\1\\" + Text + "0.jpg";
        //        string str3 = "D:\\1\\" + Text + "1.jpg";
        //        pic2.Image = Image.FromFile(str2);
        //        if (File.Exists(str3) == true)
        //        {
        //            pic3.Image = Image.FromFile(str3);
        //        }
        //        else
        //        {
        //            pic3.Image = null;
        //        }
               
        //        //这里可以换出你需要显示的字符串
        //    }
        //}

        private void dgvClass_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow currentRow = this.dgvClass.Rows[e.RowIndex];

            //获取当前行中绑定的TblClass数据对象
            clxx model = currentRow.DataBoundItem as clxx;
            if (model != null)
            {
                 sid = model.clID.ToString();
                 sctlj = model.chetlj.ToString();
                 scplj = model.cheplj.ToString();
                scphm = model.chephm.ToString();
            }
            if (File.Exists(scplj) == true)
            {
                pic3.Image = Image.FromFile(scplj);
            }
            else
            {
                pic3.Image = null;
            }
            if (File.Exists(sctlj) == true)
            {
                pic2.Image = Image.FromFile(sctlj);
            }
            else
            {
                pic2.Image = null;
            }
            textBox1.Text = scphm;
            sid = "";
            sctlj = "";
            scphm = "";
            scplj = "";
        }

        
    }
    public class ConsoleEx
    {
        /// <summary>
        /// 启动控制台
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        /// <summary>
        /// 释放控制台
        /// </summary>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();
    }


    
}
