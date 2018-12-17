namespace Tello {
	/// <summary>
	/// https://tellopilots.com/wiki/protocol/
	/// </summary>
	internal enum CommandType : ushort{// Direction
        Undefined =             0x0000,
        Connect =               0x0001, //    ->
        Connected =             0x0002, //    <-
        Query_SSID =            0x0011, //    <>
        Set_SSID =              0x0012, //    ->
        Query_Pass =            0x0013, //    ->
        Set_Pass =              0x0014, //    ->
        Query_WiFi =            0x0015, //    ->
        Set_WiFi =              0x0016, //    ->
        WiFi_Str =              0x001A, //    <-    ※たまに飛んでくる
        Set_VideoBitRate =      0x0020, //    ->
        Set_VideoDynRate =      0x0021, //    ->
        Set_Eis =               0x0024, //    ->
        Req_Video =             0x0025, //    ->
        Query_VideoBitRate =    0x0028, //    <>
        TakePicture =           0x0030, //    <>
        Set_VideoAspect =       0x0031, //    <>
        Start_Rec =             0x0032, //    ->
        EValue =                0x0034, // 
        Light_Str =             0x0035, //    <-    ※たまに飛んでくる
        Query_JpegQty =         0x0037, //    ->
        Error_1 =               0x0043, //    <-
        Error_2 =               0x0044, //    <-
        Query_Ver =             0x0045, //    <>
        Set_Date =              0x0046, //    <>
        Query_ActivTime =       0x0047, //    ->
        Query_LoaderVar =       0x0049, //    ->
        Set_Sticks =            0x0050, //    ->
        TakeOff =               0x0054, //    <>
        Land =                  0x0055, //    <>
        FlightStat =            0x0056, //    <-    ※いっぱい飛んでくる
        Set_HeightLim =         0x0058, //    ->
        Flip =                  0x005C, //    ->
        ThrowTakeOff =          0x005D, //    ->
        PalmLand =              0x005E, //    ->
        FileSize =              0x0062, //    <-
        FileData =              0x0063, //    <-
        FileDone =              0x0064, //    <-
        Start_SmartVideo =      0x0080, //    ->
        SmartVideoStat =        0x0081, //    <-
        LogHeader =             0x1050, //    <>    ※たまに飛んでくる
        LogData =               0x1051, //    <-
        LogConf =               0x1052, //    <-
        Bounce =                0x1053, //    ->
        Calibration =           0x1054, //    ->
        Set_LowBatTreshold =    0x1055, //    <>
        Query_HeightLim =       0x1056, //    <>
        Query_LowBatThreshold = 0x1057, //    <>
        Query_Attitude =        0x1058, //    ->
        Set_Attitude =          0x1059  //    ->
    }

	static class TypeEnumExt {
		public static string DisplayName(this CommandType type) {
			switch (type) {
				case CommandType.Undefined:				break;
				case CommandType.Connect:				return "Connect";
				case CommandType.Connected:				return "Connected";
				case CommandType.Query_SSID:			return "Query SSID";
				case CommandType.Set_SSID:				return "Set SSID";
				case CommandType.Query_Pass:			return "Query Password";
				case CommandType.Set_Pass:				return "Set Password";
				case CommandType.Query_WiFi:			return "Query WiFi Region";
				case CommandType.Set_WiFi:				return "Set WiFi Region";
				case CommandType.WiFi_Str:				return "WiFi Strength";
				case CommandType.Set_VideoBitRate:		return "Set Video Bit-Rate";
				case CommandType.Set_VideoDynRate:		return "Set Video Dyn. Adj. Rate";
				case CommandType.Set_Eis:				return "Set EIS";
				case CommandType.Req_Video:				return "Request Video Start";
				case CommandType.Query_VideoBitRate:	return "Query Video Bit-Rate";
				case CommandType.TakePicture:			return "Take Picture";
				case CommandType.Set_VideoAspect:		return "Set Video Aspect";
				case CommandType.Start_Rec:				return "Start Recording";
				case CommandType.EValue:				return "Exposure Values";
				case CommandType.Light_Str:				return "Light Strength";
				case CommandType.Query_JpegQty:			return "Query JPEG Quality";
				case CommandType.Error_1:				return "Error 1";
				case CommandType.Error_2:				return "Error 2";
				case CommandType.Query_Ver:				return "Query Version";
				case CommandType.Set_Date:				return "Set Date & Time";
				case CommandType.Query_ActivTime:		return "Query Activation Time";
				case CommandType.Query_LoaderVar:		return "Query Loader Version";
				case CommandType.Set_Sticks:			return "Set Sticks";
				case CommandType.TakeOff:				return "Take Off";
				case CommandType.Land:					return "Land";
				case CommandType.FlightStat:			return "Flight Status";
				case CommandType.Set_HeightLim:			return "Set Height Limit";
				case CommandType.Flip:					return "Flip";
				case CommandType.ThrowTakeOff:			return "Throw Take Off";
				case CommandType.PalmLand:				return "Palm Land";
				case CommandType.FileSize:				return "File Size";
				case CommandType.FileData:				return "File Data";
				case CommandType.FileDone:				return "File Done";
				case CommandType.Start_SmartVideo:		return "Start Smart Video";
				case CommandType.SmartVideoStat:		return "Smart Video Status";
				case CommandType.LogHeader:				return "Log Header";
				case CommandType.LogData:				return "Log Data";
				case CommandType.LogConf:				return "Log Config.";
				case CommandType.Bounce:				return "Bounce";
				case CommandType.Calibration:			return "Calibration";
				case CommandType.Set_LowBatTreshold:	return "Set Low Battery Threshold";
				case CommandType.Query_HeightLim:		return "Query Height Limit";
				case CommandType.Query_LowBatThreshold:	return "Query Low Battery Threshold";
				case CommandType.Query_Attitude:		return "Query Attitude";
				case CommandType.Set_Attitude:			return "Set Attitude";
			}
			return "Undefined";
		}
	}
}
