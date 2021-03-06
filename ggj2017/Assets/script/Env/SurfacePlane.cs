﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Env {
public abstract class SurfacePlane : MonoBehaviour, Listener {

	public Surface _surface = null;
	public float _relativePosition = 0.0f;
	public bool _animatedBottomOverride = false;
	protected bool _animatedBottom;

	public bool _frontSide = true;
	public bool _backSide = true;

	protected Vector3[] _vertices = null;
	protected Vector3[] _normals = null;
	int[] _indices = null;
	protected Vector2[] _uvs = null;

	protected int _surfaceLength = 0;
	protected float _borderDistance = 0;

	public abstract void OnUpdate();
	protected abstract void SetVertexChunk(int index);
	protected abstract int GetSize();
	protected abstract void CalculateUV(int index);
	protected abstract void Construct();

	int _chunkIndiceCount = 0;

 	void Start(){
		_surface.RegisterListener (this);
		Initialise();
	}

	void SetIndiceChunk(int index){
		int indice_ = -1;
		if (_frontSide){
			_indices [index*_chunkIndiceCount + ++indice_] = index*4;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+1;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+2;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+2;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+3;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4;
		}
		
		if (_backSide){
			_indices [index*_chunkIndiceCount + ++indice_] = index*4;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+2;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+1;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+3;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4+2;
			_indices [index*_chunkIndiceCount + ++indice_] = index*4;
		}
	}

	protected virtual bool Initialise () {
		if (_vertices != null) {
			return false;
		}

		Water _water = _surface as Water;
		_animatedBottom = _water == null ? false : (_animatedBottomOverride ? !_water._animatedBottom : _water._animatedBottom);	
		
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();
		_chunkIndiceCount = (_frontSide ? 6 : 0) + (_backSide ? 6 : 0);
		_borderDistance = (_surface._borderExtention.x + _surface.GetMetresX() * 0.5f);

		Construct();

		_surfaceLength = GetSize();
		_vertices = new Vector3[_surfaceLength * 4];
		_normals = new Vector3[_surfaceLength * 4];
		_indices = new int[_surfaceLength * _chunkIndiceCount];
		_uvs = new Vector2[_surfaceLength * 4];
		
		for (int i = 0; i < _surfaceLength; i++) {
			SetVertexChunk(i);

			SetIndiceChunk(i);
			
			CalculateUV(i);
		}

		mesh.vertices = _vertices;
		mesh.triangles = _indices;
		mesh.normals = _normals;
		mesh.uv = _uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();
		GetComponent<MeshFilter>().mesh = mesh;

		return true;
	}
}
}