<template>
  <view class="home-page">
    <!-- 用户信息区域 -->
    <view class="user-card">
      <view class="user-info">
        <uni-icons type="person-filled" size="48" color="#2979ff" />
        <view class="user-detail">
          <text class="user-name">{{ userStore.userInfo?.displayName || userStore.userInfo?.username || '未登录' }}</text>
          <text class="user-role">{{ userStore.userInfo?.role || '' }}</text>
        </view>
      </view>
    </view>

    <!-- 功能入口 -->
    <view class="section-title">快捷操作</view>
    <view class="function-grid">
      <view class="function-item" @click="navigateTo('/pages/pda/scan-in')">
        <view class="icon-wrapper icon-scan-in">
          <uni-icons type="scan" size="36" color="#ffffff" />
        </view>
        <text class="function-text">扫码入库</text>
      </view>

      <view class="function-item" @click="navigateTo('/pages/pda/scan-out')">
        <view class="icon-wrapper icon-scan-out">
          <uni-icons type="download" size="36" color="#ffffff" />
        </view>
        <text class="function-text">扫码出库</text>
      </view>

      <view class="function-item" @click="navigateTo('/pages/pda/records')">
        <view class="icon-wrapper icon-records">
          <uni-icons type="list" size="36" color="#ffffff" />
        </view>
        <text class="function-text">扫码记录</text>
      </view>

      <view class="function-item" @click="navigateTo('/pages/mine/index')">
        <view class="icon-wrapper icon-mine">
          <uni-icons type="gear-filled" size="36" color="#ffffff" />
        </view>
        <text class="function-text">个人中心</text>
      </view>
    </view>

    <!-- 使用提示 -->
    <view class="section-title">使用提示</view>
    <view class="card tips-card">
      <view class="tip-item">
        <text class="tip-number">1</text>
        <text class="tip-text">点击"扫码入库"扫描商品条形码完成入库操作</text>
      </view>
      <view class="tip-item">
        <text class="tip-number">2</text>
        <text class="tip-text">点击"扫码出库"扫描商品条形码完成出库操作</text>
      </view>
      <view class="tip-item">
        <text class="tip-number">3</text>
        <text class="tip-text">在"扫码记录"中查看所有操作历史</text>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { useUserStore } from '../../store/user'

const userStore = useUserStore()

function navigateTo(url: string) {
  if (!userStore.isLoggedIn) {
    uni.showToast({ title: '请先登录', icon: 'none' })
    setTimeout(() => {
      uni.navigateTo({ url: '/pages/login/index' })
    }, 1000)
    return
  }
  uni.navigateTo({ url })
}
</script>

<style scoped>
.home-page {
  padding: 30rpx;
  padding-bottom: 160rpx;
}

.user-card {
  background: linear-gradient(135deg, #2979ff 0%, #1a5bbf 100%);
  border-radius: 20rpx;
  padding: 40rpx;
  margin-bottom: 30rpx;
}

.user-info {
  display: flex;
  align-items: center;
}

.user-detail {
  margin-left: 24rpx;
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 36rpx;
  font-weight: bold;
  color: #ffffff;
}

.user-role {
  font-size: 24rpx;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 4rpx;
}

.section-title {
  font-size: 30rpx;
  font-weight: bold;
  color: #333;
  margin-bottom: 20rpx;
  margin-top: 10rpx;
}

.function-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 24rpx;
  margin-bottom: 30rpx;
}

.function-item {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12rpx;
}

.icon-wrapper {
  width: 108rpx;
  height: 108rpx;
  border-radius: 24rpx;
  display: flex;
  align-items: center;
  justify-content: center;
}

.icon-scan-in {
  background: linear-gradient(135deg, #18bc37, #0f9a2a);
}

.icon-scan-out {
  background: linear-gradient(135deg, #f3a73f, #e09100);
}

.icon-records {
  background: linear-gradient(135deg, #2979ff, #1a5bbf);
}

.icon-mine {
  background: linear-gradient(135deg, #9b59b6, #7d3c98);
}

.function-text {
  font-size: 24rpx;
  color: #666;
}

.tips-card {
  padding: 20rpx 30rpx;
}

.tip-item {
  display: flex;
  align-items: flex-start;
  padding: 16rpx 0;
  border-bottom: 1rpx solid #f0f0f0;
}

.tip-item:last-child {
  border-bottom: none;
}

.tip-number {
  width: 40rpx;
  height: 40rpx;
  border-radius: 50%;
  background-color: #2979ff;
  color: #ffffff;
  font-size: 22rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-right: 16rpx;
  flex-shrink: 0;
  text-align: center;
  line-height: 40rpx;
}

.tip-text {
  font-size: 26rpx;
  color: #666;
  line-height: 40rpx;
}
</style>
