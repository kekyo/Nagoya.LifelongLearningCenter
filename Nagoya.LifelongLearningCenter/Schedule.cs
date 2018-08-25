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

namespace Nagoya.LifelongLearningCenter
{
    /// <summary>
    /// Schedule informations.
    /// </summary>
    public struct Schedule
    {
        /// <summary>
        /// The center name.
        /// </summary>
        public readonly string CenterName;

        /// <summary>
        /// The room name in the center.
        /// </summary>
        public readonly string RoomName;

        /// <summary>
        /// This schedule date (contains only date, no time details).
        /// </summary>
        public readonly DateTimeOffset Date;

        /// <summary>
        /// This schedule time slot.
        /// </summary>
        public readonly TimeSlot TimeSlot;

        /// <summary>
        /// This schedule status.
        /// </summary>
        public readonly Status Status;

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="centerName">The center name</param>
        /// <param name="roomName">The room name in the center</param>
        /// <param name="date">This schedule date</param>
        /// <param name="timeSlot">This schedule time slot</param>
        /// <param name="status">This schedule status</param>
        public Schedule(string centerName, string roomName, DateTimeOffset date, TimeSlot timeSlot, Status status)
        {
            this.CenterName = centerName;
            this.RoomName = roomName;
            this.Date = date;
            this.TimeSlot = timeSlot;
            this.Status = status;
        }

        public override string ToString()
        {
            return $"{this.CenterName}, {this.RoomName}, {this.Date:d}, {this.TimeSlot}, {this.Status}";
        }
    }
}
