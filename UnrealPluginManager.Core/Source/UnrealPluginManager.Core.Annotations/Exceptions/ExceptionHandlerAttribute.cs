﻿using System;

namespace UnrealPluginManager.Core.Annotations.Exceptions;

/// <summary>
/// An attribute used to designate a class as an exception handler.
/// </summary>
/// <remarks>
/// This attribute is intended to be applied to classes that are specifically designed
/// to handle exceptions in a centralized or context-specific manner. By applying this
/// attribute, the annotated class can be recognized as an exception handler in a
/// larger framework or system architecture.
/// </remarks>
/// <seealso cref="GeneralExceptionHandlerAttribute"/>
[AttributeUsage(AttributeTargets.Class)]
public class ExceptionHandlerAttribute : Attribute;