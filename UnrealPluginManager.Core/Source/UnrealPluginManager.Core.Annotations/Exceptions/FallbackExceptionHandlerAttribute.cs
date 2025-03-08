﻿using System;

namespace UnrealPluginManager.Core.Annotations.Exceptions;

/// <summary>
/// An attribute used to mark a method as a fallback exception handler.
/// </summary>
/// <remarks>
/// The <see cref="FallbackExceptionHandlerAttribute"/> is applied to methods intended to provide
/// a fallback mechanism for handling exceptions. These handlers are typically used
/// when no specific exception handling mechanism is applicable for the encountered exception.
/// </remarks>
/// <example>
/// This attribute is applicable to methods only. Ensure the method marked with this attribute
/// adheres to the required parameter signature, generally accepting an <see cref="Exception"/> object
/// as its primary argument, to facilitate proper exception handling.
/// </example>
[AttributeUsage(AttributeTargets.Method)]
public class FallbackExceptionHandlerAttribute : Attribute;