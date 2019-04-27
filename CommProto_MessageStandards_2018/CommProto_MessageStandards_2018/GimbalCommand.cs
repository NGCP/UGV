/*
 GimbalCommand
 

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
   * GimbalCommand Packet Template. 
   */
  public class GimbalCommand : ABSPacket
  {
    public GimbalCommand(UInt16 theta = 0,
      UInt16 phi = 0)
      : base("GimbalCommand")
    {
      this.theta = theta;
      this.phi = phi;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(theta);
      obj.Input(phi);
    }

    public override void Unpack(ObjectStream obj)
    {
      phi = obj.OutputUInt16();
      theta = obj.OutputUInt16();
    }
	
    public override ABSPacket Create()
    {
      return new GimbalCommand();
    }

    #region Data
    public UInt16 theta { get; set; }
    public UInt16 phi { get; set; }
    #endregion
  }
}

