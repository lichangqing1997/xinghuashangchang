<script setup lang="ts">
import { onLaunch } from '@dcloudio/uni-app'
import { useUserStore } from './store/user'

// 不需要登录即可访问的页面
const whiteList = ['/pages/login/index']

onLaunch(() => {
  console.log('App Launch')
  const userStore = useUserStore()
  userStore.loadToken()

  // 路由拦截器：未登录时自动跳转登录页
  uni.addInterceptor('navigateTo', { before: checkLogin })
  uni.addInterceptor('redirectTo', { before: checkLogin })
  uni.addInterceptor('switchTab', { before: checkLogin })
  uni.addInterceptor('reLaunch', { before: checkLogin })
})

function checkLogin(options: { url: string }) {
  const path = options.url.split('?')[0]
  if (whiteList.includes(path)) {
    return true
  }
  const token = uni.getStorageSync('xinghua_token')
  if (!token) {
    uni.reLaunch({ url: '/pages/login/index' })
    return false
  }
  return true
}
</script>

<style>
page {
  background-color: #f5f5f5;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
  font-size: 28rpx;
  color: #333333;
}

.container {
  padding: 30rpx;
}

.btn-primary {
  background-color: #2979ff;
  color: #ffffff;
  border-radius: 12rpx;
  height: 88rpx;
  line-height: 88rpx;
  font-size: 32rpx;
  text-align: center;
  border: none;
}

.btn-primary:active {
  opacity: 0.8;
}

.card {
  background-color: #ffffff;
  border-radius: 16rpx;
  padding: 30rpx;
  margin-bottom: 24rpx;
  box-shadow: 0 2rpx 12rpx rgba(0, 0, 0, 0.05);
}
</style>
