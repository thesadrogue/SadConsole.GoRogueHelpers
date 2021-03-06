﻿using System;

namespace GoRogue.Random
{
    /// <summary>
    /// Simple implementation of the random generator used by GoRogue. This one is mapped to the <see cref="SadConsole.Global.Random"/> instance.
    /// </summary>
    public sealed class SadConsoleRandomGenerator : Troschuetz.Random.Generators.AbstractGenerator
    {
        private readonly byte[] _uintBuffer;

        public SadConsoleRandomGenerator() : base(0) => _uintBuffer = new byte[4];

        public override int NextInclusiveMaxValue() => SadConsole.Global.Random.Next();

        public override double NextDouble() => SadConsole.Global.Random.NextDouble();

        public override uint NextUIntInclusiveMaxValue()
        {
            SadConsole.Global.Random.NextBytes(_uintBuffer);
            return BitConverter.ToUInt32(_uintBuffer, 0);
        }

        public override bool CanReset => false;
    }
}
