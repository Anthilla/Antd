///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.ViewHelpers {
    public class VHStatus {
        public static List<StatusSysctlViewModel> Sysctl(List<SysctlModel> stockData, List<SysctlModel> runningData, List<SysctlModel> antdData) {
            HashSet<string> paramNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { };
            foreach (SysctlModel data in stockData) {
                paramNames.Add(data.param);
            }
            foreach (SysctlModel data in runningData) {
                paramNames.Add(data.param);
            }
            foreach (SysctlModel data in antdData) {
                paramNames.Add(data.param);
            }
            List<StatusSysctlViewModel> list = new List<StatusSysctlViewModel>() { };
            foreach (string par in paramNames) {
                var model = new StatusSysctlViewModel();
                model.label = par;

                var stockValue = (from s in stockData
                                  where s.param == par
                                  select s.value).FirstOrDefault();
                if (stockValue == null) {
                    model.stockValue = "";
                }
                else {
                    model.stockValue = stockValue;
                }

                var runningValue = (from s in runningData
                                  where s.param == par
                                  select s.value).FirstOrDefault();
                if (runningValue == null) {
                    model.runningValue = "";
                }
                else {
                    model.runningValue = runningValue;
                }

                var antdValue = (from s in antdData
                                  where s.param == par
                                  select s.value).FirstOrDefault();
                if (antdValue == null) {
                    model.antdValue = "";
                }
                else {
                    model.antdValue = antdValue;
                }
                list.Add(model);
            }
            //list = (from l in list
            //        where l != null
            //        orderby l.label ascending
            //        select l).ToList();
			return list;
        }
    }
}
