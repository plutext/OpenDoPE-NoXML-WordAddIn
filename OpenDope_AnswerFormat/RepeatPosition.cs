﻿/*
 *  OpenDoPE authoring Word AddIn
    Copyright (C) Plutext Pty Ltd, 2012
 * 
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

using OpenDoPEModel;

namespace OpenDope_AnswerFormat
{
    class RepeatPosition
    {
        public question Repeat { get; set; }

        public RepeatPosition(question q)
        {
            Repeat = q;
        }

        public override string ToString()
        {
            return "Position in repeat '" + Repeat.text + "'";
        }

    }
}
