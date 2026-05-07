// Copyright (c) LumexUI 2024
// LumexUI licenses this file to you under the MIT license
// See the license here https://github.com/LumexUI/lumexui/blob/main/LICENSE

using System.Diagnostics.CodeAnalysis;

using LumexUI.Common;
using LumexUI.Utilities;

namespace LumexUI.Styles;

[ExcludeFromCodeCoverage]
internal readonly record struct Divider
{
    private readonly static string _base = ElementClass.Empty()
        .Add( "bg-divider" )
        .Add( "border-none" )
        .ToString();

    private static ElementClass GetOrientationStyles( Orientation orientation )
    {
        return ElementClass.Empty()
            .Add( "w-full h-px", when: orientation is Orientation.Horizontal )
            .Add( "h-full w-px", when: orientation is Orientation.Vertical );
    }

    public static string GetStyles( LumexDivider divider )
    {
        var styles = new ElementClass()
            .Add( _base )
            .Add( GetOrientationStyles( divider.Orientation ) )
            .Add( divider.Class )
            .ToString();

        return styles;
    }
}
