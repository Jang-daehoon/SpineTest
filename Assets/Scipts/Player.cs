using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�� ��ũ��Ʈ ������ �ִϸ��̼��� ����Ǵ� ���ۿ� �ش��ϴ� ������ �̷������.
//Idle->Walk ���� ū ������ ����ȴ�.
namespace SpineTest
{
    public class Player : MonoBehaviour
    {
        #region public Field
        //������ �ִϸ��̼��� ���� ��
        public SkeletonAnimation skeletonAnimation;
        public AnimationReferenceAsset[] animClip;

        //�ִϸ��̼ǿ� ���� ENUM
        public enum AnimState
        {
            IDLE, BLINK
        }
        #endregion
        #region private Field
        //���� �ִϸ��̼� ó���� ���������� ���� ����
        private AnimState animState;
        private string currentAnimation;    //���� � �ִϸ��̼��� ����ǰ� �ִ����� ���� ����

        //Moveó��
        private Rigidbody2D rig;
        private float xx;
        #endregion

        private void Awake() => rig = GetComponent<Rigidbody2D>();

        private void Update()
        {
            xx = Input.GetAxisRaw("Horizontal");

            if (xx == 0f)
                animState = AnimState.IDLE;
            else
            {
                animState = AnimState.BLINK;
                transform.localScale = new Vector2(xx, 1);
            }

            //�ִϸ��̼� ����
            SetCurrentAnimation(animState);

        }
        private void FixedUpdate()=> rig.velocity = new Vector2(xx * 300 * Time.deltaTime, rig.velocity.y);

        private void _AsncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
        {
            if (animClip.name.Equals(currentAnimation)) return; //�� �����Ӹ��� �������� �ʵ��� ����.

            //�ش� �ִϸ��̼����� �����Ѵ�.
            skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
            skeletonAnimation.loop = loop;
            skeletonAnimation.timeScale = timeScale;

            //���� ����ǰ� �ִ� �ִϸ��̼� ���� ����.
            currentAnimation = animClip.name;
        }

        private void SetCurrentAnimation(AnimState _state)
        {
            switch(_state)
            {
                case AnimState.IDLE:
                    _AsncAnimation(animClip[(int)AnimState.IDLE], true, 1f);
                    break;
                case AnimState.BLINK:
                    _AsncAnimation(animClip[(int)AnimState.BLINK], true, 1f);
                    break;
            }
            //ª�� ������ �ڵ�
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }

    }
}
