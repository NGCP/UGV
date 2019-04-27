/*
 SpeedSteeringCommand
 

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
   * SpeedSteeringCommand Packet Template. 
   */
  public class SpeedSteeringCommand : ABSPacket
  {
    public SpeedSteeringCommand(UInt16 speed = 0,
      UInt16 steering = 0)
      : base("SpeedSteeringCommand")
    {
      this.speed = speed;
      this.steering = steering;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(speed);
      obj.Input(steering);
    }

    public override void Unpack(ObjectStream obj)
    {
      steering = obj.OutputUInt16();
      speed = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new SpeedSteeringCommand();
    }

    #region Data
    public UInt16 speed { get; set; }
    public UInt16 steering { get; set; }
    #endregion
  }
}

