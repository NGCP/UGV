/*
 VehicleInertialState
 

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
   * VehicleInertialState Packet Template. 
   */
  public class VehicleInertialState : ABSPacket
  {
    public VehicleInertialState(UInt16 vehicle_id = 0,
      Single longitude = 0,
      Single latitude = 0,
      Single altitude = 0,
      Single roll = 0,
      Single pitch = 0,
      Single heading = 0,
      Single north_speed = 0,
      Single east_speed = 0,
      Single vertical_speed = 0,
      Single roll_rate = 0,
      Single pitch_rate = 0,
      Single yaw_rate = 0,
      Single north_accel = 0,
      Single east_accel = 0,
      Single vertical_accel = 0)
      : base("VehicleInertialState")
    {
      this.vehicle_id = vehicle_id;
      this.longitude = longitude;
      this.latitude = latitude;
      this.altitude = altitude;
      this.roll = roll;
      this.pitch = pitch;
      this.heading = heading;
      this.north_speed = north_speed;
      this.east_speed = east_speed;
      this.vertical_speed = vertical_speed;
      this.roll_rate = roll_rate;
      this.pitch_rate = pitch_rate;
      this.yaw_rate = yaw_rate;
      this.north_accel = north_accel;
      this.east_accel = east_accel;
      this.vertical_accel = vertical_accel;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(longitude);
      obj.Input(latitude);
      obj.Input(altitude);
      obj.Input(roll);
      obj.Input(pitch);
      obj.Input(heading);
      obj.Input(north_speed);
      obj.Input(east_speed);
      obj.Input(vertical_speed);
      obj.Input(roll_rate);
      obj.Input(pitch_rate);
      obj.Input(yaw_rate);
      obj.Input(north_accel);
      obj.Input(east_accel);
      obj.Input(vertical_accel);
    }

    public override void Unpack(ObjectStream obj)
    {
      vertical_accel = obj.OutputSingle();
      east_accel = obj.OutputSingle();
      north_accel = obj.OutputSingle();
      yaw_rate = obj.OutputSingle();
      pitch_rate = obj.OutputSingle();
      roll_rate = obj.OutputSingle();
      vertical_speed = obj.OutputSingle();
      east_speed = obj.OutputSingle();
      north_speed = obj.OutputSingle();
      heading = obj.OutputSingle();
      pitch = obj.OutputSingle();
      roll = obj.OutputSingle();
      altitude = obj.OutputSingle();
      latitude = obj.OutputSingle();
      longitude = obj.OutputSingle();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleInertialState();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    //radians
    public Single longitude { get; set; }
    //radians
    public Single latitude { get; set; }
    //meters
    public Single altitude { get; set; }
    //radians
    public Single roll { get; set; }
    //radians
    public Single pitch { get; set; }
    //radians
    public Single heading { get; set; }
    //meters/second
    public Single north_speed { get; set; }
    //meters/second
    public Single east_speed { get; set; }
    //meters/second
    public Single vertical_speed { get; set; }
    //radians/second
    public Single roll_rate { get; set; }
    //radians/second
    public Single pitch_rate { get; set; }
    //radians/second
    public Single yaw_rate { get; set; }
    //meters/second/second
    public Single north_accel { get; set; }
    //meters/second/second
    public Single east_accel { get; set; }
    //meters/second/second
    public Single vertical_accel { get; set; }
    #endregion
  }
}

