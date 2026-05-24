<template>
  <view class="login-page">
    <view class="logo-section">
      <image class="logo" src="/static/tab-home.png" mode="aspectFit" />
      <text class="title">兴华商城</text>
      <text class="subtitle">仓储管理系统</text>
    </view>

    <view class="form-section">
      <view class="input-group">
        <uni-icons type="person" size="20" color="#999" />
        <input
          v-model="username"
          type="text"
          placeholder="请输入用户名"
          class="input"
          @confirm="handleLogin"
        />
      </view>

      <view class="input-group">
        <uni-icons type="locked" size="20" color="#999" />
        <input
          v-model="password"
          type="text"
          :password="!showPassword"
          placeholder="请输入密码"
          class="input"
          @confirm="handleLogin"
        />
        <uni-icons
          :type="showPassword ? 'eye' : 'eye-slash'"
          size="20"
          color="#999"
          @click="showPassword = !showPassword"
        />
      </view>

      <button class="login-btn" :loading="loading" @click="handleLogin">
        登 录
      </button>
    </view>

    <view class="footer">
      <text class="footer-text">默认账号: admin / admin123</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '../../store/user'

const userStore = useUserStore()
const username = ref('admin')
const password = ref('admin123')
const showPassword = ref(false)
const loading = ref(false)

async function handleLogin() {
  if (!username.value.trim()) {
    uni.showToast({ title: '请输入用户名', icon: 'none' })
    return
  }
  if (!password.value.trim()) {
    uni.showToast({ title: '请输入密码', icon: 'none' })
    return
  }

  loading.value = true
  try {
    await userStore.login(username.value, password.value)
    uni.showToast({ title: '登录成功', icon: 'success' })
    setTimeout(() => {
      uni.switchTab({ url: '/pages/index/index' })
    }, 1000)
  } catch (e) {
    // 错误已由 request.ts 处理
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #2979ff 0%, #1a5bbf 100%);
  padding: 60rpx;
}

.logo-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 80rpx;
}

.logo {
  width: 160rpx;
  height: 160rpx;
  margin-bottom: 24rpx;
  background-color: #ffffff;
  border-radius: 32rpx;
}

.title {
  font-size: 48rpx;
  font-weight: bold;
  color: #ffffff;
}

.subtitle {
  font-size: 28rpx;
  color: rgba(255, 255, 255, 0.8);
  margin-top: 8rpx;
}

.form-section {
  width: 100%;
  background-color: #ffffff;
  border-radius: 24rpx;
  padding: 50rpx 40rpx;
  box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.1);
}

.input-group {
  display: flex;
  align-items: center;
  border: 2rpx solid #e5e5e5;
  border-radius: 12rpx;
  padding: 0 24rpx;
  height: 96rpx;
  margin-bottom: 30rpx;
  background-color: #f8f8f8;
}

.input-group:last-of-type {
  margin-bottom: 0;
}

.input {
  flex: 1;
  height: 96rpx;
  font-size: 30rpx;
  margin-left: 16rpx;
  color: #333;
}

.login-btn {
  width: 100%;
  height: 96rpx;
  line-height: 96rpx;
  background-color: #2979ff;
  color: #ffffff;
  font-size: 34rpx;
  font-weight: bold;
  border-radius: 12rpx;
  margin-top: 50rpx;
  border: none;
}

.login-btn:active {
  opacity: 0.85;
}

.footer {
  margin-top: 40rpx;
}

.footer-text {
  font-size: 24rpx;
  color: rgba(255, 255, 255, 0.7);
}
</style>
