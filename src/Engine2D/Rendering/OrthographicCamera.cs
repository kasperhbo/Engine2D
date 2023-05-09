﻿using Engine2D.GameObjects;
using OpenTK.Mathematics;
using System.Text.Json;
using System.Xml.Linq;

namespace Engine2D.Rendering;

internal class OrthographicCamera : Gameobject
{
    internal Matrix4 ProjectionMatrix { get; private set; }
    internal Color4 ClearColor { get; } = new(56, 61, 58, 255);
    private Matrix4 _viewMatrix = Matrix4.Identity;

    private float _aspectRatio;
    private float _size;

    private float _near = -1f;
    private float _far = 1f;

    internal bool IsMainCamera = true;

    internal System.Numerics.Vector2 Position = new(0,0);

    internal Matrix4 ViewMatrix { 
        get { 
            CalculateViewMatrix(); 
            return _viewMatrix; 
        } 
    }

    public Matrix4 InverseView { get; private set; }
    public Matrix4 InverseProjection { get; private set; } = new();

    private readonly Vector3 _front = -Vector3.UnitZ;
    private readonly Vector3 _up = Vector3.UnitY;
    private void CalculateViewMatrix()
    {
        _viewMatrix = Matrix4.LookAt(new Vector3(Position.X, Position.Y, 0), new Vector3(Position.X, Position.Y, 0) + _front, _up);
        InverseView = Matrix4.Invert(_viewMatrix);
    }      


    public OrthographicCamera(float aspectRatio, float size)
    {
        this._aspectRatio = aspectRatio;
        this._size = size;
        UpdateProjectionMatrix();
    }

    public void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4.CreateOrthographicOffCenter(-_size * _aspectRatio, _size * _aspectRatio, -_size, _size, -1f, 1f);
        InverseProjection = Matrix4.Invert(ProjectionMatrix);
    }

    public void SetAspectRatio(float aspectRatio)
    {
        this._aspectRatio = aspectRatio;
        UpdateProjectionMatrix();
    }

    public void SetSize(float size)
    {
        this._size = size;
        UpdateProjectionMatrix();
    }

}