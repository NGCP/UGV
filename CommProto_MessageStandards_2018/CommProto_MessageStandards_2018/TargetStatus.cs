/*
 TargetStatus
 

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
   * TargetStatus Packet Template. 
   */
  public class TargetStatus : ABSPacket
  {
    public TargetStatus(Double target_radius = 0,
      Double target_angle = 0,
      Double target_altitude = 0)
      : base("TargetStatus")
    {
      this.target_radius = target_radius;
      this.target_angle = target_angle;
      this.target_altitude = target_altitude;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(target_radius);
      obj.Input(target_angle);
      obj.Input(target_altitude);
    }

    public override void Unpack(ObjectStream obj)
    {
      target_altitude = obj.OutputDouble();
      target_angle = obj.OutputDouble();
      target_radius = obj.OutputDouble();
    }
	
    public override ABSPacket Create()
    {
      return new TargetStatus();
    }

    #region Data
    //kilometers
    public Double target_radius { get; set; }
    //radians
    public Double target_angle { get; set; }
    //meters
    public Double target_altitude { get; set; }
    #endregion
  }
}

