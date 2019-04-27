/*
 ArmCommand
 

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
   * ArmCommand Packet Template. 
   */
  public class ArmCommand : ABSPacket
  {
    public ArmCommand(Byte id = 0,
      Int32 position = 0)
      : base("ArmCommand")
    {
      this.id = id;
      this.position = position;
    }

    public override void Pack(ObjectStream obj)
    {
      obj.Input(id);
      obj.Input(position);
    }

    public override void Unpack(ObjectStream obj)
    {
      position = obj.OutputInt32();
      id = obj.OutputByte();
    }
	
    public override ABSPacket Create()
    {
      return new ArmCommand();
    }

    #region Data
    public Byte id { get; set; }
    public Int32 position { get; set; }
    #endregion
  }
}

