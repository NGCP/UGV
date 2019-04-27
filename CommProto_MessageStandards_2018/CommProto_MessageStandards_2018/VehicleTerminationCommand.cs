/*
 VehicleTerminationCommand
 

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
   * VehicleTerminationCommand Packet Template. 
   */
  public class VehicleTerminationCommand : ABSPacket
  {
    public VehicleTerminationCommand(UInt16 vehicle_id = 0,
      Byte termination_mode = 0)
      : base("VehicleTerminationCommand")
    {
      this.vehicle_id = vehicle_id;
      this.termination_mode = termination_mode;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(vehicle_id);
      obj.Input(termination_mode);
    }

    public override void Unpack(ObjectStream obj)
    {
      termination_mode = obj.OutputByte();
      vehicle_id = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new VehicleTerminationCommand();
    }

    #region Data
    public UInt16 vehicle_id { get; set; }
    public Byte termination_mode { get; set; }
    #endregion
  }
}

