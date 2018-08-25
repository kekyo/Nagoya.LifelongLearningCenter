/*
 * Nagoya LifelongLearningCenter information fetcher.
 * Copyright (c) 2018 Kouji Matsui, All rights reserved.
 * https://github.com/kekyo/Nagoya.LifelongLearningCenter
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Xml.Linq;

namespace Nagoya.LifelongLearningCenter
{
    public sealed class FetchingEventArgs : EventArgs
    {
        public readonly Uri Url;

        public FetchingEventArgs(Uri url)
        {
            this.Url = url;
        }
    }

    public delegate void FetchingEventDelegate(object sender, FetchingEventArgs e);

    public sealed class FetchedEventArgs : EventArgs
    {
        public readonly Uri Url;
        public readonly XDocument Document;
        public readonly Exception Exception;

        public FetchedEventArgs(Uri url, XDocument document, Exception ex)
        {
            this.Url = url;
            this.Document = document;
            this.Exception = ex;
        }
    }

    public delegate void FetchedEventDelegate(object sender, FetchedEventArgs e);
}
