/*
 VehicleBodySensedState
 

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
   * VehicleBodySensedState Packet Template. 
   */
  public class VehicleBodySensedState : ABSPacket
  {
    public VehicleBodySensedState(UInt16 vehicle_id = 0,
      Single x_accel = 0,
      Single y_accel = 0,
      Single z_accel = 0,
      Single roll_rate = 0,
      Single pitch_rate = 0,
      Single yaw_rate = 0)
      : base("VehicleBodySensedState")
    {
      this.vehicle_id = vehicle_id;
      this.x_accel = x_accel;
      this.y_accel = y_accel;
      this.z_accel = z_accel;
      this.roll_rate = roll_rate;
      this.pitch_rate = pitch_rate;
      this.yaw_rate = yaw_rate;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(x_accel);
      obj.Input(y_accel);
      obj.Input(z_accel);
      obj.Input(roll_rate);
      obj.Input(pitch_rate);
      obj.Input(yaw_rate);
    }

    public override void Unpack(ObjectStream obj)
    {
      yaw_rate = obj.OutputSingle();
      pitch_rate = obj.OutputSingle();
      roll_rate = obj.OutputSingle();
      z_accel = obj.OutputSingle();
      y_accel = obj.OutputSingle();
      x_accel = obj.OutputSingle();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleBodySensedState();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    //meters/second/second
    public Single x_accel { get; set; }
    //meters/second/second
    public Single y_accel { get; set; }
    //meters/second/second
    public Single z_accel { get; set; }
    //radians/second
    public Single roll_rate { get; set; }
    //radians/second
    public Single pitch_rate { get; set; }
    //radians/second
    public Single yaw_rate { get; set; }
    #endregion
  }
}

