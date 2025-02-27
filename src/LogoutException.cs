// Copyright (C) 2013 Dmitry Yakimenko (detunized@gmail.com).
// Licensed under the terms of the MIT license. See LICENCE for details.

using System;

namespace SkuVault.LastPass
{
    public class LogoutException: BaseException
    {
        public enum FailureReason
        {
            WebException
        }

        public LogoutException(FailureReason reason, string message): base(message)
        {
            Reason = reason;
        }

        public LogoutException(FailureReason reason, string message, Exception innerException):
            base(message, innerException)
        {
            Reason = reason;
        }

        public FailureReason Reason { get; private set; }
    }
}
