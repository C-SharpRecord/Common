using System;
using System.Collections.Generic;
using System.Text;

namespace Common.UniqueIdGenerator
{
    /// <summary>
    /// snowflake
    /// 64-bits 
    /// ------------------------------------------------------------------------------
    /// | 0 | 41-bit timestamp | 5-bit DatacenterId | 5-bit WorkId | 12-bit Sequence |
    /// ------------------------------------------------------------------------------
    /// </summary>
    public class UniqueGenerator
    {
        public long WorkId { get; private set; }
        public long DatacenterId { get; private set; }
        public long Sequence { get; private set; }


        private static readonly long workerIdBits = 5;
        private static readonly long datacenterIdBits = 5;
        private static readonly long sequenceBits = 12;

        private static readonly long maxWorkerId = -1L ^ (-1L << (int)workerIdBits);
        private static readonly long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);

        private readonly long workerIdShift = sequenceBits;
        private readonly long datacenterIdShift = sequenceBits + workerIdBits;
        private readonly long timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;
        private readonly long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

        private long lastTimestamp = -1L;

        private static readonly object locker = new object();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="workId">workId，(system、business..)(5 bit)(0-31)</param>
        /// <param name="datacenterId">datacenterId, machine id(5 bit)(0-31)</param>
        /// <param name="sequence">sequence (12 bit)(0-4095)</param>
        public UniqueGenerator(long workId, long datacenterId, long sequence)
        {
            if (workId > maxWorkerId || workId < 1)
            {
                throw new ArgumentException($"{nameof(workId)} can't be greater than {maxWorkerId} or less than 1");
            }
            if (datacenterId > maxDatacenterId || datacenterId < 1)
            {
                throw new ArgumentException($"{nameof(datacenterId)} can't be greater than {maxWorkerId} or less than 1");
            }

            //todo
            //can print parameter logs

            WorkId = workId;
            DatacenterId = datacenterId;
            Sequence = sequence;
        }

        public long GetNextId()
        {
            lock (locker)
            {

                long timestamp = GetTimestamp();

                if (lastTimestamp == timestamp)
                {
                    Sequence = (Sequence + 1) & sequenceMask;
                    if (Sequence == 0)
                    {
                        timestamp = GetNextTimestamp(lastTimestamp);
                    }
                }
                else
                {
                    Sequence = 0;
                }

                lastTimestamp = timestamp;

                return ((timestamp) << (int)timestampLeftShift) |
                        (DatacenterId << (int)datacenterIdShift) |
                        (WorkId << (int)workerIdShift) |
                        Sequence;
            }
        }

        private long GetTimestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        private long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            while (timestamp <= lastTimestamp)
            {
                timestamp = GetTimestamp();
            }
            return timestamp;
        }

    }
}
