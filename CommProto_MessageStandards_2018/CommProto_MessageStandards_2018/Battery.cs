/*
 Battery
 

 Copyright (C) 2016-2017  Northrup Grumman Collaboration Project.
 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
  
 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
  
 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Comnet;
using Comnet.Serialization;

namespace NGCP
{
  /**
   * Battery Packet Template. 
   */
  public class Battery : ABSPacket
  {
    public Battery(UInt32 batteryPercentage = 0)
      : base("Battery")
    {
      this.batteryPercentage = batteryPercentage;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(batteryPercentage);
    }

    public override void Unpack(ObjectStream obj)
    {
      batteryPercentage = obj.OutputUInt32();
    }
	
    public override ABSPacket Create()
    {
      return new Battery();
    }

    #region Data
    public UInt32 batteryPercentage { get; set; }
    #endregion
  }
}

