using System;

namespace SadConsole.Actions
{
    /// <summary>
    /// Base class for actions that provide a success and failure callback.
    /// </summary>
    public abstract class ActionBase
    {
        /// <summary>
        /// Runs when this action is completed with a successful result.  <see cref="OnSuccessResult"/> is not called if this function
        /// returns false.
        /// </summary>
        public Func<ActionBase, bool> OnSuccessMethod;

        /// <summary>
        /// Runs when this action is completed with a failure result.  <see cref="OnFailureResult"/> is not called if this function
        /// returns false.
        /// </summary>
        public Func<ActionBase, bool> OnFailureMethod;

        /// <summary>
        /// When <see langword="true"/>, indicates that this action has been completed; otherwise <see langword="false"/>
        /// </summary>
        public bool IsFinished { get; private set; }

        /// <summary>
        /// A success or failure result for this action.
        /// </summary>
        public ActionResult Result { get; private set; } = ActionResult.Failure;

        /// <summary>
        /// Finishes the action with the specified result.
        /// </summary>
        /// <param name="result"></param>
        public void Finish(ActionResult result)
        {
            Result = result;
            IsFinished = true;

            if (Result.IsSuccess && (OnSuccessMethod?.Invoke(this) ?? true))
                OnSuccessResult();
            else if (OnFailureMethod?.Invoke(this) ?? true)
                OnFailureResult();
        }

        public abstract void Run(TimeSpan timeElapsed);

        /// <summary>
        /// Called when the Action has produced a successful result.
        /// </summary>
        protected virtual void OnSuccessResult() { }

        protected virtual void OnFailureResult() { }
    }

    public abstract class ActionBase<TSource, TTarget> : ActionBase
    {
        public TSource Source { get; protected set; }
        public TTarget Target { get; protected set; }

        public ActionBase(TSource source, TTarget target)
        {
            Source = source;
            Target = target;
        }
    }
}
