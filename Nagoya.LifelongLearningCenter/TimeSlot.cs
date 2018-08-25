﻿/*
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

namespace Nagoya.LifelongLearningCenter
{
    /// <summary>
    /// Schedule time slots.
    /// </summary>
    /// <remarks>Time slot defines by lerning center, see the site descriptions.</remarks>
    public enum TimeSlot
    {
        /// <summary>
        /// Time slot is in the morning.
        /// </summary>
        Morning = 0,

        /// <summary>
        /// Time slot is afternoon.
        /// </summary>
        Afternoon = 1,

        /// <summary>
        /// Time slot is in the evening.
        /// </summary>
        Evening = 2
    }
}
