<template>
  <view class="mine-page">
    <!-- 用户头像区域 -->
    <view class="user-header">
      <view class="avatar-wrapper">
        <uni-icons type="person-filled" size="60" color="#ffffff" />
      </view>
      <view class="user-detail">
        <text class="user-name">{{ userStore.userInfo?.displayName || userStore.userInfo?.username || '未登录' }}</text>
        <text class="user-role">{{ userStore.userInfo?.role || '请先登录' }}</text>
      </view>
    </view>

    <!-- 功能列表 -->
    <view class="card menu-card">
      <view class="menu-item" @click="navigateTo('/pages/pda/scan-in')">
        <uni-icons type="scan" size="22" color="#2979ff" />
        <text class="menu-text">扫码入库</text>
        <uni-icons type="right" size="16" color="#ccc" />
      </view>
      <view class="menu-item" @click="navigateTo('/pages/pda/scan-out')">
        <uni-icons type="download" size="22" color="#f3a73f" />
        <text class="menu-text">扫码出库</text>
        <uni-icons type="right" size="16" color="#ccc" />
      </view>
      <view class="menu-item" @click="navigateTo('/pages/pda/records')">
        <uni-icons type="list" size="22" color="#18bc37" />
        <text class="menu-text">扫码记录</text>
        <uni-icons type="right" size="16" color="#ccc" />
      </view>
    </view>

    <!-- 退出登录 -->
    <view v-if="userStore.isLoggedIn" class="logout-section">
      <button class="logout-btn" @click="handleLogout">退出登录</button>
    </view>
  </view>
</template>

<script setup lang="ts">
import { useUserStore } from '../../store/user'

const userStore = useUserStore()

function navigateTo(url: string) {
  uni.navigateTo({ url })
}

function handleLogout() {
  uni.showModal({
    title: '提示',
    content: '确定要退出登录吗？',
    success: async (res) => {
      if (res.confirm) {
        await userStore.logout()
        uni.showToast({ title: '已退出登录', icon: 'success' })
        setTimeout(() => {
          uni.reLaunch({ url: '/pages/login/index' })
        }, 1000)
      }
    }
  })
}
</script>

<style scoped>
.mine-page {
  min-height: 100vh;
  background-color: #f5f5f5;
}

.user-header {
  background: linear-gradient(135deg, #2979ff 0%, #1a5bbf 100%);
  padding: 60rpx 40rpx;
  display: flex;
  align-items: center;
}

.avatar-wrapper {
  width: 120rpx;
  height: 120rpx;
  border-radius: 50%;
  background-color: rgba(255, 255, 255, 0.2);
  display: flex;
  align-items: center;
  justify-content: center;
}

.user-detail {
  margin-left: 30rpx;
  display: flex;
  flex-direction: column;
}

.user-name {
  font-size: 36rpx;
  font-weight: bold;
  color: #ffffff;
}

.user-role {
  font-size: 26rpx;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 8rpx;
}

.menu-card {
  margin: 30rpx;
  padding: 0;
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 30rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.menu-item:last-child {
  border-bottom: none;
}

.menu-text {
  flex: 1;
  font-size: 30rpx;
  color: #333;
  margin-left: 20rpx;
}

.logout-section {
  padding: 60rpx 30rpx;
}

.logout-btn {
  width: 100%;
  height: 88rpx;
  line-height: 88rpx;
  background-color: #ffffff;
  color: #e43d33;
  font-size: 30rpx;
  border-radius: 12rpx;
  border: 2rpx solid #e43d33;
}

.logout-btn:active {
  opacity: 0.85;
}
</style>
