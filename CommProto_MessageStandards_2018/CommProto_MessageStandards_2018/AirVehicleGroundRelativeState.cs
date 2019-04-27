/*
 AirVehicleGroundRelativeState
 

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
   * AirVehicleGroundRelativeState Packet Template. 
   */
  public class AirVehicleGroundRelativeState : ABSPacket
  {
    public AirVehicleGroundRelativeState(UInt16 vehicle_id = 0,
      Single angle_of_attack = 0,
      Single angle_of_sideslip = 0,
      Single true_airspeed = 0,
      Single indicated_airspeed = 0,
      Single north_wind_speed = 0,
      Single east_wind_speed = 0,
      Single north_ground_speed = 0,
      Single east_ground_speed = 0,
      Single barometric_pressure = 0,
      Single barometric_altitude = 0)
      : base("AirVehicleGroundRelativeState")
    {
      this.vehicle_id = vehicle_id;
      this.angle_of_attack = angle_of_attack;
      this.angle_of_sideslip = angle_of_sideslip;
      this.true_airspeed = true_airspeed;
      this.indicated_airspeed = indicated_airspeed;
      this.north_wind_speed = north_wind_speed;
      this.east_wind_speed = east_wind_speed;
      this.north_ground_speed = north_ground_speed;
      this.east_ground_speed = east_ground_speed;
      this.barometric_pressure = barometric_pressure;
      this.barometric_altitude = barometric_altitude;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(angle_of_attack);
      obj.Input(angle_of_sideslip);
      obj.Input(true_airspeed);
      obj.Input(indicated_airspeed);
      obj.Input(north_wind_speed);
      obj.Input(east_wind_speed);
      obj.Input(north_ground_speed);
      obj.Input(east_ground_speed);
      obj.Input(barometric_pressure);
      obj.Input(barometric_altitude);
    }

    public override void Unpack(ObjectStream obj)
    {
      barometric_altitude = obj.OutputSingle();
      barometric_pressure = obj.OutputSingle();
      east_ground_speed = obj.OutputSingle();
      north_ground_speed = obj.OutputSingle();
      east_wind_speed = obj.OutputSingle();
      north_wind_speed = obj.OutputSingle();
      indicated_airspeed = obj.OutputSingle();
      true_airspeed = obj.OutputSingle();
      angle_of_sideslip = obj.OutputSingle();
      angle_of_attack = obj.OutputSingle();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new AirVehicleGroundRelativeState();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    //radians
    public Single angle_of_attack { get; set; }
    //radians
    public Single angle_of_sideslip { get; set; }
    //meters/second
    public Single true_airspeed { get; set; }
    //meters/second
    public Single indicated_airspeed { get; set; }
    //meters/second
    public Single north_wind_speed { get; set; }
    //meters/second
    public Single east_wind_speed { get; set; }
    //meters/second
    public Single north_ground_speed { get; set; }
    //meters/second
    public Single east_ground_speed { get; set; }
    //pascals
    public Single barometric_pressure { get; set; }
    //meters
    public Single barometric_altitude { get; set; }
    #endregion
  }
}

