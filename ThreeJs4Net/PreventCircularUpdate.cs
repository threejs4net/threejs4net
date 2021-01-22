using System;
using System.Collections.Generic;

namespace ThreeJs4Net
{
    internal class PreventCircularUpdate
    {
        #region Fields

        private readonly Stack<bool> _stack = new Stack<bool>();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// </summary>
        public PreventCircularUpdate(object somethingToLockWith = null)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        private bool CanIgnore { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// </summary>
        /// <param name="action"></param>
        public void Do(Action action)
        {
            if (null == action) return;

            if (this.CanIgnore)
                return; // prevent circular update

            this.Push(true); // save previous value and raise to true

            action();

            this.Pop(); // restores previous value
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        private void Pop()
        {
            this.CanIgnore = this._stack.Pop();
        }

        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        private void Push(bool value)
        {
            this._stack.Push(this.CanIgnore);

            this.CanIgnore = value;
        }

        #endregion
    }
}