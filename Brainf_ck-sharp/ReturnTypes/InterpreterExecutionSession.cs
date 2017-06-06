using System;
using System.Collections.Generic;
using System.Text;

namespace Brainf_ck_sharp.ReturnTypes
{
    public sealed class InterpreterExecutionSession
    {
        public InterpreterResult CurrentResult { get; }

        public InterpreterExecutionSession Continue() => Brainf_ckInterpreter.ContinueSession(Clone());

        public InterpreterExecutionSession RunToCompletion() => Brainf_ckInterpreter.RunSessionToCompletion(Clone());

        public bool CanContinue => CurrentResult.HasFlag(InterpreterExitCode.Success) &&
                                   CurrentResult.HasFlag(InterpreterExitCode.BreakpointReached);

        internal SessionDebugData DebugData { get; }

        internal InterpreterExecutionSession(InterpreterResult result, SessionDebugData data)
        {
            CurrentResult = result;
            DebugData = data;
        }

        public InterpreterExecutionSession Clone()
        {
            InterpreterResult result = CurrentResult.Clone();
            SessionDebugData data = DebugData.Clone();
            return new InterpreterExecutionSession(result, data);
        }
    }
}
