/*
 VehicleAttitude
 

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
   * VehicleAttitude Packet Template. 
   */
  public class VehicleAttitude : ABSPacket
  {
    public VehicleAttitude(UInt16 vehicle_id = 0,
      Single roll = 0,
      Single pitch = 0,
      Single yaw = 0)
      : base("VehicleAttitude")
    {
      this.vehicle_id = vehicle_id;
      this.roll = roll;
      this.pitch = pitch;
      this.yaw = yaw;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(roll);
      obj.Input(pitch);
      obj.Input(yaw);
    }

    public override void Unpack(ObjectStream obj)
    {
      yaw = obj.OutputSingle();
      pitch = obj.OutputSingle();
      roll = obj.OutputSingle();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleAttitude();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    //radians
    public Single roll { get; set; }
    //radians
    public Single pitch { get; set; }
    //radians
    public Single yaw { get; set; }
    #endregion
  }
}

