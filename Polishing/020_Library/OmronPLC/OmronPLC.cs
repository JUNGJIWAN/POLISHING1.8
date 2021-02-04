using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Generic;

namespace mcOMRON
{
	/// <summary>
	/// Implemented transport's type
	/// </summary>
	public enum TransportType
	{
		Tcp
	};
	/// <summary>
	/// 
	/// Version:	1.0
	/// Author:		Joan Magnet
	/// Date:		02/2015
	/// 
	/// manage communications with an OMRON PLC
	/// 
	/// Implemented FINS commands:
	/// 
	///		- [1,1] MEMORY AREA READ
	///		- [1,2] MEMORY AREA WRITE
	///		- [5,1] CONTROLLER DATA READ
	/// 
	/// Specific DM commands:
	/// 
	///		- ReadDM		(read one DM)
	///		- ReadDMs		(read various DM)
	///		- WriteDM		(write one DM)
	///		- ClearDMs		(clear, set to 0 various DM)
	///		- ReadCIOBit	(read 1 bit from CIO area)
	///		- WriteCioBit	(write one bit at CIO area)
	/// 
	/// </summary>
	public class OmronPLC
	{
		#region **** properties

		/// <summary>
		/// return the connection status
		/// </summary>
		public bool Connected
		{
			get { return this._finsCmd.Connected; }
		}


		/// <summary>
		/// last detected error
		/// </summary>
		public string LastError
		{
			get { return this._finsCmd.LastError; }
		}


		/// <summary>
		/// current FINS command object
		/// </summary>
		public mcOMRON.IFINSCommand FinsCommand
		{
			get { return this._finsCmd; }
		}

		public byte[] _bReadE0Data { get {return this.m_ReadE0Data; } }

		public byte[] _bReadE2Data { get { return this.m_ReadE2Data; } }

		public byte[] _CommandData { get {return this._finsCmd.Command; } }

		public byte[] _ReceiveData { get {return this._finsCmd.Response; } }

		public int _QueueStep { get {return this.m_nSendStep; } }

		#endregion

		#region **** variables
		private int m_nSendStep = 0;
		private byte[] m_ReadE0Data = new byte[512]; // 32 Address * 16 Byte
		private byte[] m_ReadE2Data = new byte[4]  ; // 4 Byte

		#region **** FINS command

		// FINS command object
		//
		private mcOMRON.IFINSCommand _finsCmd;

        #endregion

        #endregion

        #region **** constructor

        /// <summary>
        /// constructor, a IFinsCommand layer required
        /// </summary>
        /// <param name="finsCommand"></param>
        public OmronPLC(TransportType TType = TransportType.Tcp)
		{
			switch(TType)
			{
				case TransportType.Tcp:
					this._finsCmd = new tcpFINSCommand();
					break;
				default:
					throw new Exception("Transport type not defined.");
			}
		}

		#endregion

		#region **** connect & close

		/// <summary>
		/// try to connect with the plc
		/// </summary>
		public bool Connect()
		{
			return this._finsCmd.Connect();
		}



		/// <summary>
		/// close the communication with the plc
		/// </summary>
		public void Close()
		{
			this._finsCmd.Close();
		}

		#endregion

		#region **** FINS commands

		/// <summary>
		/// 
		/// MEMORY AREA READ
		/// 
		/// </summary>
		public bool finsMemoryAreadRead(MemoryArea area, UInt16 address, Byte bit_position, UInt16 count)
		{
			// area = EM , address = 0, bit_position = 0, count 2048
			return _finsCmd.MemoryAreaRead(area, address, bit_position, count);
		}



		/// <summary>
		/// 
		/// MEMORY AREA WRITE
		/// 
		/// </summary>
		public bool finsMemoryAreadWrite(MemoryArea area, UInt16 address, Byte bit_position, UInt16 count, Byte[] data)
		{
			// area = ET , address = 0, bit_position = 가변, count 1, data = 00 or 01
			return _finsCmd.MemoryAreaWrite(area, address, bit_position, count, ref data);
		}



		/// <summary>
		/// 
		/// CONNECTION DATA READ
		/// 
		/// </summary>
		/// <param name="area"></param>
		/// <returns></returns>
		public bool finsConnectionDataRead(Byte area)
		{
			return _finsCmd.ConnectionDataRead(area);
		}

		#endregion

		#region **** predefined DM commands

		/// <summary>
		/// read one DM
		/// </summary>
		/// <param name="position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ReadDM(UInt16 position, ref UInt16 value)
		{
			// FINS command
			//
			if (!finsMemoryAreadRead(MemoryArea.DM, position, 0, 1)) return false;

			// value
			//
			value = BTool.BytesToUInt16(this._finsCmd.Response[0], this._finsCmd.Response[1]);

			return true;
		}



		/// <summary>
		/// read one DM using signed values
		/// </summary>
		/// <param name="position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ReadDM(UInt16 position, ref Int16 value)
		{
			// FINS command
			//
			if (!finsMemoryAreadRead(MemoryArea.DM, position, 0, 1)) return false;

			// value
			//
			value = BTool.BytesToInt16(this._finsCmd.Response[0], this._finsCmd.Response[1]);

			return true;
		}



		/// <summary>
		/// read various DM
		/// </summary>
		/// <param name="position"></param>
		/// <param name="data"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public bool ReadDMs(UInt16 position, ref UInt16[] data, UInt16 count)
		{
			// FINS command
			//
			if (!finsMemoryAreadRead(MemoryArea.DM, position, 0, count)) return false;

			// fills the array
			//
			for (int x = 0; x < count; x++)
			{
				data[x] = BTool.BytesToUInt16(this._finsCmd.Response[(x * 2)], this._finsCmd.Response[(x * 2) + 1]);
			}

			return true;
		}



		/// <summary>
		/// write one DM
		/// </summary>
		/// <param name="position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool WriteDM(UInt16 position, UInt16 value)
		{
			// get the array
			//
			Byte[] cmd = BTool.Uint16toBytes(value);

			// fins command
			//
			return finsMemoryAreadWrite(MemoryArea.DM, position, 0, 1, cmd);
		}



		/// <summary>
		/// write one DM
		/// </summary>
		/// <param name="position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool WriteDM(UInt16 position, Int16 value)
		{
			// get the array
			//
			Byte[] cmd = BTool.Int16toBytes(value);

			// fins command
			//
			return finsMemoryAreadWrite(MemoryArea.DM, position, 0, 1, cmd);
		}



		/// <summary>
		/// fills with 0 a mamory area of the PLC
		/// </summary>
		/// <param name="position"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public bool ClearDMs(UInt16 position, UInt16 count)
		{
			// zeroed array (each DM requieres 2 bytes)
			//
			Byte[] cmd = new Byte[count * 2];
			for (int x = 0; x < (count * 2); cmd[x++] = 0) ;

			// fins command
			//
			return finsMemoryAreadWrite(MemoryArea.DM, position, 0, count, cmd);
		}

		#endregion

		#region **** predefined CIO commands

		/// <summary>
		/// reads an specifit bit of CIO area
		/// </summary>
		/// <param name="position"></param>
		/// <param name="bit_position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool ReadCIOBit(UInt16 position, Byte bit_position, ref Byte value)
		{
			// FINS command
			//
			if (!finsMemoryAreadRead(MemoryArea.CIO_Bit, position, bit_position, 1)) return false;

			// value
			//
			//value = BTool.BytesToUInt16(this._finsCmd.Response[0], this._finsCmd.Response[1]);
			value = this._finsCmd.Response[0];

			return true;
		}



		/// <summary>
		/// write one specific bit of CIO area
		/// </summary>
		/// <param name="position"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool WriteCIOBit(UInt16 position, Byte bit_position, Byte value)
		{
			// get the array
			//
			Byte[] cmd = new Byte[1];
			cmd[0] = value;

			// fins command
			//
			return finsMemoryAreadWrite(MemoryArea.CIO_Bit, position, bit_position, 1, cmd);
		}

		#endregion

		#region	**** Slurry Auto Supply commands
		//-------------------------------------------------------------------------------------------------
		/**    
		<summary>
		    Set Parameter (IP, Port)
		</summary>
		<param name="strIp">IP 주소</param>
		<param name="nPort">Port No.</param>
		@author    이준호(LEEJOONHO)
		@date      2020/03/16 15:35
		*/
		public bool fn_SetParam(string strIp, int nPort)
		{
			try
			{
                mcOMRON.tcpFINSCommand tcpCommand = ((mcOMRON.tcpFINSCommand)this.FinsCommand);
                tcpCommand.SetTCPParams(IPAddress.Parse(strIp), Convert.ToInt32(nPort));
				return true;
            }
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				return false;
			}
        }
		
		//-------------------------------------------------------------------------------------------------
		/**    
		<summary>
		    Read Auto Supply Memory Area (E0/E2)
		</summary>
		<param name="area">EM = E0 Area, ET = E2 Area</param>
		<param name="address">Address 주소</param>
		<param name="byte_position">Request Byte Position</param>
		<param name="bArray">Receive Data Array from PLC</param>
		<param name="count">Request Byte Count</param>
		@author    이준호(LEEJOONHO)
		@date      2020/03/16 11:48
		*/
		public bool fn_ReadArea(MemoryArea area, UInt16 address, byte byte_position, UInt16 count)
		{
			if (area != MemoryArea.EM && area != MemoryArea.ET) return false;
			if (address < 0) return false;
			
			try
			{
				if (!finsMemoryAreadRead(area, address, byte_position, count)) return false;
				else
				{
					if (area == MemoryArea.EM)
					{
                        m_ReadE0Data = this._finsCmd.Response;
                        return true;
                    }
					else
					{
                        m_ReadE2Data = this._finsCmd.Response;
                        return true;
                    }
                }
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				return false;
			}
		}
		//-------------------------------------------------------------------------------------------------
		/**    
		<summary>
		    Write Auto Supply Memory Area (E2)
		</summary>
		<param name="address">Address 주소</param>
		<param name="byte_position">Request Byte Position</param>
		<param name="count">Request Byte Count</param>
		<param name="bOn">True 0x01, False 0x00</param>
		@author    이준호(LEEJOONHO)
		@date      2020/03/16 11:54
		*/
		public bool fn_SendArea(MemoryArea area, UInt16 address, byte byte_position, UInt16 count, bool bOn)
		{
			if (area != MemoryArea.ET) return false;
			
			byte[] byte_value = new byte[1];

			byte_value[0] = Convert.ToByte(bOn);

			if (byte_value[0] != 0 && byte_value[0] != 1) return false;
			
			try
			{
				if (!finsMemoryAreadWrite(area, address, byte_position, count, byte_value)) return false;
				else
				{
					m_ReadE2Data = this._finsCmd.Response;
					return true;
				}
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				return false;
			}
		}
        //-------------------------------------------------------------------------------------------------
        /**    
		<summary>
		    Write Auto Supply Memory Area (E2)
		</summary>
		<param name="address">Address 주소</param>
		<param name="byte_position">Request Byte Position</param>
		<param name="count">Request Byte Count</param>
		<param name="bOn">True 0x01, False 0x00</param>
		@author    이준호(LEEJOONHO)
		@date      2020/03/16 11:54
		*/
        public bool fn_SendArea(ushort add, byte[] set)
        {
            bool brtn = false;
            try
            {
                brtn = finsMemoryAreadWrite(MemoryArea.ET, add, 0, 4, set);

                if (brtn) m_ReadE2Data = this._finsCmd.Response;

                return brtn;

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return false;
            }
        }
        //-------------------------------------------------------------------------------------------------	
        #endregion

        #region **** dialog

        /// <summary>
        /// return last dialog between PC & PLC
        /// </summary>
        /// <param name="Caption"></param>
        /// <returns></returns>
        public string LastDialog(string Caption)
		{
			return this.FinsCommand.LastDialog(Caption);
		}
		
		#endregion		

	}
}

