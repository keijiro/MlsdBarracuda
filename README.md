MlsdBarracuda
=============

![screenshot](https://user-images.githubusercontent.com/343936/124577416-33265200-de88-11eb-9cc4-652e4324abef.png)

**MlsdBarracuda** is a Unity sample project that runs the line segment
detection model ([M-LSD]) on the [Barracuda] neural network inference engine.

[M-LSD]: https://github.com/navervision/mlsd
[Barracuda]: https://docs.unity3d.com/Packages/com.unity.barracuda@latest

The M-LSD model was developed by NAVER/LINE Vision. See the
[original repository][M-LSD] and [paper] for further details.

[paper]: https://arxiv.org/abs/2106.00186

ONNX model file
---------------

The original M-LSD model was provided as a .tflite file. [PINTO0309]
(Katsuya Hyodo) converted it into ONNX. I re-converted it into a
Barracuda-compatible form using [this Colab notebook].

[PINTO0309]: https://github.com/PINTO0309/PINTO_model_zoo
[this Colab notebook]: 
  https://colab.research.google.com/drive/1MBUcs4On26MCF_saKcqR00yz_FFzGGno?usp=sharing

Related Projects
----------------

- [MlsdVfx] - An experimental project using M-LSD for visual effects.

[MlsdVfx]: https://github.com/keijiro/MlsdVfx
