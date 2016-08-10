﻿using CSharpGL;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RendererGenerator
{
    class RendererBuilder
    {
        public string GetFilename(DataStructure dataStructure)
        {
            return string.Format("{0}.cs", dataStructure.RendererName);
        }

        public void Build(DataStructure dataStructure, string rendererFilename = "")
        {
            if (string.IsNullOrEmpty(rendererFilename)) { rendererFilename = this.GetFilename(dataStructure); }

            var rendererType = new CodeTypeDeclaration(dataStructure.RendererName);
            rendererType.IsClass = true;
            rendererType.IsPartial = true;
            rendererType.BaseTypes.Add(typeof(Renderer));
            rendererType.Comments.Add(new CodeCommentStatement("<summary>", true));
            rendererType.Comments.Add(new CodeCommentStatement(string.Format("Renderer of {0}", dataStructure.TargetName), true));
            rendererType.Comments.Add(new CodeCommentStatement("</summary>", true));
            BuildCreate(rendererType, dataStructure);
            BuildConstructor(rendererType, dataStructure);
            BuildDoInitialize(rendererType, dataStructure);
            BuildDoRender(rendererType, dataStructure);

            var parserNamespace = new CodeNamespace("CSharpGL");
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Object).Namespace));
            parserNamespace.Imports.Add(new CodeNamespaceImport(typeof(System.Collections.Generic.List<int>).Namespace));
            parserNamespace.Types.Add(rendererType);

            //生成代码  
            using (var stream = new StreamWriter(rendererFilename, false))
            {
                CSharpCodeProvider codeProvider = new CSharpCodeProvider();
                CodeGeneratorOptions opentions = new CodeGeneratorOptions();//代码生成选项
                opentions.BlankLinesBetweenMembers = true;
                opentions.BracingStyle = "C";
                opentions.ElseOnClosing = false;
                opentions.IndentString = "    ";
                opentions.VerbatimOrder = true;

                codeProvider.GenerateCodeFromNamespace(parserNamespace, stream, opentions);
            }
        }

        private void BuildDoRender(CodeTypeDeclaration rendererType, DataStructure dataStructure)
        {
            //throw new NotImplementedException();
        }

        private void BuildDoInitialize(CodeTypeDeclaration rendererType, DataStructure dataStructure)
        {
            //throw new NotImplementedException();
        }

        private void BuildConstructor(CodeTypeDeclaration rendererType, DataStructure dataStructure)
        {
            //throw new NotImplementedException();
        }

        private void BuildCreate(CodeTypeDeclaration rendererType, DataStructure dataStructure)
        {
            CodeMemberMethod method = CreateDeclaration(dataStructure);
            CreateBody(method, dataStructure);
            rendererType.Members.Add(method);
        }

        private void CreateBody(CodeMemberMethod method, DataStructure dataStructure)
        {
            {
                // var shaderCodes = new ShaderCode[2];
                method.Statements.Add(new CodeSnippetStatement(string.Format("            var {0} = new {1}[2];", shaderCodes, typeof(ShaderCode).Name)));
                method.Statements.Add(new CodeSnippetStatement(string.Format("            {0}[0] = new {1}(File.ReadAllText(@\"shaders\\{2}.vert\"), {3}.{4});", shaderCodes, typeof(ShaderCode).Name, dataStructure.TargetName, ShaderType.VertexShader.GetType().Name, ShaderType.VertexShader)));
                method.Statements.Add(new CodeSnippetStatement(string.Format("            {0}[0] = new {1}(File.ReadAllText(@\"shaders\\{2}.frag\"), {3}.{4});", shaderCodes, typeof(ShaderCode).Name, dataStructure.TargetName, ShaderType.VertexShader.GetType().Name, ShaderType.FragmentShader)));
            }
            {
                // var map = new PropertyNameMap();
                method.Statements.Add(new CodeSnippetStatement("            var map = new PropertyNameMap();"));
                // map.Add("in_Position", GroundModel.strPosition);
                foreach (var item in dataStructure.PropertyList)
                {
                    method.Statements.Add(new CodeMethodInvokeExpression(
                        new CodeVariableReferenceExpression("map"), "Add",
                        new CodePrimitiveExpression(item.NameInShader),
                        new CodeSnippetExpression(string.Format("{0}.{1}",
                            dataStructure.ModelName, item.NameInModel))));
                }
            }
            {
                // var renderer = new GroundRenderer(model, shaderCodes, map);
                method.Statements.Add(new CodeSnippetStatement(string.Format("            var renderer = new {0}({1}, {2}, map);", dataStructure.RendererName, model, shaderCodes)));
            }
            {
                // return renderer;
                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("renderer")));
            }
        }

        private CodeMemberMethod CreateDeclaration(DataStructure dataStructure)
        {
            var method = new CodeMemberMethod();
            method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(dataStructure.RendererName);
            method.Name = "Create";
            var parameter0 = new CodeParameterDeclarationExpression(dataStructure.ModelName, model);
            method.Parameters.Add(parameter0);

            return method;
        }

        private const string model = "model";
        private const string shaderCodes = "shaderCodes";
    }
}
