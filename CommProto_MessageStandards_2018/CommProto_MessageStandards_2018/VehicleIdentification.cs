/*
 VehicleIdentification
 

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
   * VehicleIdentification Packet Template. 
   */
  public class VehicleIdentification : ABSPacket
  {
    public VehicleIdentification(UInt16 vehicle_id = 0,
      Byte vehicle_type = 0)
      : base("VehicleIdentification")
    {
      this.vehicle_id = vehicle_id;
      this.vehicle_type = vehicle_type;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(vehicle_type);
    }

    public override void Unpack(ObjectStream obj)
    {
      vehicle_type = obj.OutputByte();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleIdentification();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    public Byte vehicle_type { get; set; }
    #endregion
  }
}

