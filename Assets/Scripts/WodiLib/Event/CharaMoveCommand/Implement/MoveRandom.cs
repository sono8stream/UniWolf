// ========================================
// Project Name : WodiLib
// File Name    : MoveRandom.cs
//
// MIT License Copyright(c) 2019 kameske
// see LICENSE file
// ========================================

using System;
using System.ComponentModel;
using WodiLib.Project;

namespace WodiLib.Event.CharaMoveCommand
{
    /// <inheritdoc />
    /// <summary>
    /// 動作指定：ランダム移動
    /// </summary>
    [Serializable]
    public class MoveRandom : CharaMoveCommandBase
    {
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     Private Constant
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        private const string EventCommandSentenceFormat = "ランダム移動";

        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/
        //     OverrideMethod
        // _/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/_/

        /// <inheritdoc />
        public override CharaMoveCommandCode CommandCode => CharaMoveCommandCode.MoveRandom;

        /// <inheritdoc />
        public override byte ValueLengthByte => 0x00;

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string GetEventCommandSentence(
            EventCommandSentenceResolver resolver, EventCommandSentenceType type,
            EventCommandSentenceResolveDesc desc)
        {
            return EventCommandSentenceFormat;
        }
    }
}