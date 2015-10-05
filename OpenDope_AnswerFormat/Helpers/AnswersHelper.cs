/*
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

using NLog;

using OpenDoPEModel;

namespace OpenDope_AnswerFormat.Helpers
{
    class AnswersHelper
    {
        static Logger log = LogManager.GetLogger("OpenDoPE_Wed");

        public static repeat findRepeat(answers answers, string id)
        {
            return findRepeat(answers.Items, id);
        }

        private static repeat findRepeat(List<object> objects, string id)
        {
            {
                foreach (object o in objects)
                {
                    if (o is answer)
                    {
                        // do nothing
                    }
                    else if (o is repeat)
                    {
                        repeat r = (repeat)o;

                        if (r.qref.Equals(id))
                        {
                            return r;
                        }
                        else
                        {
                            repeat rFound = findRepeat(r.row[0].Items, id);
                            if (rFound != null) return rFound;
                        }
                    }
                }
                return null;
            }

        }
    }
}