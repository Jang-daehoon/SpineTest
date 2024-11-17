using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

//Blink�� Idle���� ����ǵ��� �ۼ�
namespace SpineTest
{
    public class PlayerMixedAnim : MonoBehaviour
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
        private float minimumDelay = 0.15f;
        private float maximumDelay = 3f;
        #endregion

        private void Awake() => rig = GetComponent<Rigidbody2D>();

        private void Start()
        {
            //Idle���¿� �������� ��� �߰�
            StartCoroutine(_AsncFaceChange(animClip[(int)AnimState.BLINK], false, 1f));
        }
        private void Update()
        {
            xx = Input.GetAxisRaw("Horizontal");

            if(xx != 0f)
            {
                transform.localScale = new Vector2(xx, 1);
            }

            //�ִϸ��̼� ����
            SetCurrentAnimation(animState);

            if (Input.GetKeyDown(KeyCode.Space)) // ��: �ǰ� ��Ȳ
            {
                ChangeFaceOne(animClip[(int)AnimState.BLINK], false, 1f, animClip[(int)AnimState.IDLE]);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                ChangeFaceOnly(animClip[(int)AnimState.BLINK], false, 1f, 1);
            }

        }
        private void FixedUpdate() => rig.velocity = new Vector2(xx * 300 * Time.deltaTime, rig.velocity.y);

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

        //ǥ���� ��ȭ�� �ִٸ�
        private IEnumerator _AsncFaceChange(AnimationReferenceAsset animClip, bool loop, float timeScale, int track = 1)
        {
            if (animClip.name.Equals(currentAnimation)) yield break; //�� �����Ӹ��� �������� �ʵ��� ����.
           
            while(true)
            {
                skeletonAnimation.state.SetAnimation(track, animClip, loop).TimeScale = timeScale;  //1�� Ʈ������ ���
                yield return new WaitForSeconds(Random.Range(minimumDelay, maximumDelay));
            }
        }
        //Stage�� ���� �����ϴ� �ִϸ��̼��� �������ش�.
        private void SetCurrentAnimation(AnimState _state)
        {
            switch (_state)
            {
                case AnimState.IDLE:
                    _AsncAnimation(animClip[(int)AnimState.IDLE], true, 1f);
                    break;
            }
            //ª�� ������ �ڵ�
            //_AsncAnimation(animClip[(int)_state],true, 1f);  

        }
        //�Ͻ����� �ǰݰ� ���� ��Ȳ�� ǥ���� �ٲ���� 1ȸ ��� �� ���� ���·� �����ؾ��Ѵ�.
        private void ChangeFaceOne(AnimationReferenceAsset animClip, bool loop, float timeScale, AnimationReferenceAsset defaultAnimClip, int track = 1)
        {
            // ������ �ִϸ��̼� ���
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, animClip, loop);
            entry.TimeScale = timeScale;

            // �ִϸ��̼� ���� �� �⺻ ���·� ����
            entry.Complete += (trackEntry) =>
            {
                skeletonAnimation.state.SetAnimation(track, defaultAnimClip, true).TimeScale = 1f; // �⺻ �ִϸ��̼����� ����
            };
        }
        //ǥ���� �����Ϸ���?
        private void ChangeFaceOnly(AnimationReferenceAsset faceAnimClip, bool loop, float timeScale, int track = 1)
        {
            // ������ Ʈ��(1�� Ʈ��)���� �ִϸ��̼� ���
            TrackEntry entry = skeletonAnimation.state.SetAnimation(track, faceAnimClip, loop);
            entry.TimeScale = timeScale;
            print($"ǥ�� ��ȭ! ��ȭ���� Ŭ��{faceAnimClip.name}");
            // ǥ�� �ִϸ��̼��� ����Ǹ� �ش� Ʈ���� ���ų� ����
            if (!loop)
            {
                entry.Complete += (trackEntry) =>
                {
                    skeletonAnimation.state.ClearTrack(track); // ǥ�� �ִϸ��̼��� ������ Ʈ���� ���
                };
            }
        }

    }
}
