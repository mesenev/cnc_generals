// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Proto/annotations.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Google.Api {

  /// <summary>Holder for reflection information generated from Proto/annotations.proto</summary>
  public static partial class AnnotationsReflection {

    #region Descriptor
    /// <summary>File descriptor for Proto/annotations.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static AnnotationsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdQcm90by9hbm5vdGF0aW9ucy5wcm90bxIKZ29vZ2xlLmFwaRoQUHJvdG8v",
            "aHR0cC5wcm90bxogZ29vZ2xlL3Byb3RvYnVmL2Rlc2NyaXB0b3IucHJvdG86",
            "RQoEaHR0cBIeLmdvb2dsZS5wcm90b2J1Zi5NZXRob2RPcHRpb25zGLDKvCIg",
            "ASgLMhQuZ29vZ2xlLmFwaS5IdHRwUnVsZUJuCg5jb20uZ29vZ2xlLmFwaUIQ",
            "QW5ub3RhdGlvbnNQcm90b1ABWkFnb29nbGUuZ29sYW5nLm9yZy9nZW5wcm90",
            "by9nb29nbGVhcGlzL2FwaS9hbm5vdGF0aW9uczthbm5vdGF0aW9uc6ICBEdB",
            "UEliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Api.HttpReflection.Descriptor, global::Google.Protobuf.Reflection.DescriptorReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, new pb::Extension[] { AnnotationsExtensions.Http }, null));
    }
    #endregion

  }
  /// <summary>Holder for extension identifiers generated from the top level of Proto/annotations.proto</summary>
  public static partial class AnnotationsExtensions {
    /// <summary>
    /// See `HttpRule`.
    /// </summary>
    public static readonly pb::Extension<global::Google.Protobuf.Reflection.MethodOptions, global::Google.Api.HttpRule> Http =
      new pb::Extension<global::Google.Protobuf.Reflection.MethodOptions, global::Google.Api.HttpRule>(72295728, pb::FieldCodec.ForMessage(578365826, global::Google.Api.HttpRule.Parser));
  }

}

#endregion Designer generated code