﻿using System;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CustomFunctions
{
    private const float DEFAULT_FRAME_AMOUNT = 1 / 60f;
    private const float FIXED_FRAME_AMOUNT = 1 / 50f;

    private const int MAX_COMPONENT_SEARCH_DEPTH = 20;

    public static TComponent GetComponentInParent<TComponent>(
        this Component mb,
        int maxDepth = MAX_COMPONENT_SEARCH_DEPTH
    )
    {
        var cItem = mb.transform;

        for (var i = 0; i < maxDepth && cItem != null; i++)
        {
            // Try to get the component
            var found = cItem.TryGetComponent(out TComponent component);

            // If there is a component, return it
            if (found)
                return component;

            // Move to the parent of the current item
            cItem = cItem.transform.parent;
        }

        // There is none of the component in any of the parents
        return default;
    }

    public static bool TryGetComponentInParent<TComponent>(
        this Component mb,
        out TComponent component,
        int maxDepth = MAX_COMPONENT_SEARCH_DEPTH
    )
    {
        var cItem = mb.transform;

        for (var i = 0; i < maxDepth && cItem != null; i++)
        {
            // Try to get the component
            var found = cItem.TryGetComponent(out component);

            // If there is a component, return it
            if (found)
                return true;

            // Move to the parent of the current item
            cItem = cItem.transform.parent;
        }

        // There is none of the component in any of the parents
        component = default;

        return false;
    }

    public static float FrameAmount(float lerpAmount, bool isFixed = false, bool isUnscaled = false)
    {
        var frameAmount = isFixed ? FIXED_FRAME_AMOUNT : DEFAULT_FRAME_AMOUNT;

        float deltaTime;

        if (isUnscaled)
            deltaTime = isFixed ? Time.fixedUnscaledDeltaTime : Time.unscaledDeltaTime;
        else
            deltaTime = isFixed ? Time.fixedDeltaTime : Time.deltaTime;

        return deltaTime / frameAmount * lerpAmount;
    }
}