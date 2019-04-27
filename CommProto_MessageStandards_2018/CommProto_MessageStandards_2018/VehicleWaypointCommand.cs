/*
 VehicleWaypointCommand
 

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
   * VehicleWaypointCommand Packet Template. 
   */
  public class VehicleWaypointCommand : ABSPacket
  {
    public VehicleWaypointCommand(UInt16 vehicle_id = 0,
      Single longitude = 0,
      Single latitude = 0,
      Single altitude = 0)
      : base("VehicleWaypointCommand")
    {
      this.vehicle_id = vehicle_id;
      this.longitude = longitude;
      this.latitude = latitude;
      this.altitude = altitude;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(longitude);
      obj.Input(latitude);
      obj.Input(altitude);
    }

    public override void Unpack(ObjectStream obj)
    {
      altitude = obj.OutputSingle();
      latitude = obj.OutputSingle();
      longitude = obj.OutputSingle();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleWaypointCommand();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    //radians
    public Single longitude { get; set; }
    //radians
    public Single latitude { get; set; }
    //meters
    public Single altitude { get; set; }
    #endregion
  }
}

