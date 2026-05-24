<template>
  <view class="scan-page">
    <!-- 扫码按钮 -->
    <view class="scan-section">
      <view class="scan-btn" @click="handleScan">
        <uni-icons type="scan" size="60" color="#ffffff" />
        <text class="scan-btn-text">点击扫码</text>
      </view>
      <!-- #ifdef H5 -->
      <text class="scan-hint">点击按钮手动输入条形码</text>
      <!-- #endif -->
      <!-- #ifndef H5 -->
      <text class="scan-hint">扫描商品条形码自动填入</text>
      <!-- #endif -->
    </view>

    <!-- 商品信息展示 -->
    <view v-if="product" class="card product-card">
      <view class="product-header">
        <text class="product-name">{{ product.商品名称 }}</text>
        <uni-tag :text="product.类别" type="primary" size="small" />
      </view>
      <view class="product-info">
        <view class="info-row">
          <text class="info-label">条形码</text>
          <text class="info-value">{{ product.条形码 }}</text>
        </view>
        <view class="info-row">
          <text class="info-label">编码</text>
          <text class="info-value">{{ product.商品编码 }}</text>
        </view>
        <view class="info-row">
          <text class="info-label">颜色/尺码</text>
          <text class="info-value">{{ product.颜色 || '-' }} / {{ product.尺码 || '-' }}</text>
        </view>
      </view>
    </view>

    <!-- 表单 -->
    <view class="card form-card">
      <view class="form-item">
        <text class="form-label">条形码 <text class="required">*</text></text>
        <input
          v-model="form.条形码"
          placeholder="请输入或扫描条形码"
          class="form-input"
          @blur="onBarcodeBlur"
        />
      </view>

      <view class="form-item">
        <text class="form-label">库位编码 <text class="required">*</text></text>
        <input
          v-model="form.库位编码"
          placeholder="请输入库位编码，如 A-1-1"
          class="form-input"
        />
      </view>

      <view class="form-item">
        <text class="form-label">数量 <text class="required">*</text></text>
        <uni-number-box v-model="form.数量" :min="1" :max="9999" />
      </view>

      <view class="form-item">
        <text class="form-label">进货单号</text>
        <input
          v-model="form.进货单号"
          placeholder="可选，关联进货单"
          class="form-input"
        />
      </view>

      <view class="form-item">
        <text class="form-label">操作人</text>
        <input
          v-model="form.操作人"
          placeholder="可选"
          class="form-input"
        />
      </view>
    </view>

    <!-- 提交按钮 -->
    <button class="submit-btn" :loading="submitting" @click="handleSubmit">
      确认入库
    </button>
  </view>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import { getProductByBarcode, type Product } from '../../api/product'
import { scanIn } from '../../api/pda'

const product = ref<Product | null>(null)
const submitting = ref(false)

const form = reactive({
  条形码: '',
  库位编码: '',
  数量: 1,
  进货单号: '',
  操作人: ''
})

function handleScan() {
  // #ifdef H5
  uni.showModal({
    title: '输入条形码',
    editable: true,
    placeholderText: '请输入条形码',
    success: (res) => {
      if (res.confirm && res.content) {
        form.条形码 = res.content.trim()
        lookupProduct(res.content.trim())
      }
    }
  })
  // #endif

  // #ifndef H5
  uni.scanCode({
    scanType: ['barCode'],
    success: (res) => {
      form.条形码 = res.result
      lookupProduct(res.result)
    },
    fail: () => {
      uni.showToast({ title: '扫码取消', icon: 'none' })
    }
  })
  // #endif
}

async function onBarcodeBlur() {
  if (form.条形码.trim()) {
    await lookupProduct(form.条形码.trim())
  }
}

async function lookupProduct(barcode: string) {
  try {
    product.value = await getProductByBarcode(barcode)
  } catch {
    product.value = null
  }
}

async function handleSubmit() {
  if (!form.条形码.trim()) {
    uni.showToast({ title: '请输入条形码', icon: 'none' })
    return
  }
  if (!form.库位编码.trim()) {
    uni.showToast({ title: '请输入库位编码', icon: 'none' })
    return
  }

  submitting.value = true
  try {
    const msg = await scanIn({
      条形码: form.条形码.trim(),
      库位编码: form.库位编码.trim(),
      数量: form.数量,
      进货单号: form.进货单号 || undefined,
      操作人: form.操作人 || undefined
    })
    uni.showToast({ title: msg || '入库成功', icon: 'success' })
    // 重置表单
    form.条形码 = ''
    form.库位编码 = ''
    form.数量 = 1
    form.进货单号 = ''
    product.value = null
  } catch {
    // 错误已由 request.ts 处理
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.scan-page {
  padding: 30rpx;
  padding-bottom: 50rpx;
}

.scan-section {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 30rpx;
}

.scan-btn {
  width: 240rpx;
  height: 240rpx;
  border-radius: 50%;
  background: linear-gradient(135deg, #2979ff, #1a5bbf);
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  box-shadow: 0 8rpx 24rpx rgba(41, 121, 255, 0.3);
}

.scan-btn:active {
  transform: scale(0.95);
}

.scan-btn-text {
  color: #ffffff;
  font-size: 28rpx;
  margin-top: 8rpx;
}

.scan-hint {
  font-size: 24rpx;
  color: #999;
  margin-top: 16rpx;
}

.product-card {
  margin-bottom: 24rpx;
}

.product-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 20rpx;
}

.product-name {
  font-size: 32rpx;
  font-weight: bold;
  color: #333;
}

.product-info {
  background-color: #f8f9fa;
  border-radius: 12rpx;
  padding: 20rpx;
}

.info-row {
  display: flex;
  justify-content: space-between;
  padding: 8rpx 0;
}

.info-label {
  font-size: 26rpx;
  color: #999;
}

.info-value {
  font-size: 26rpx;
  color: #333;
}

.form-card {
  margin-bottom: 30rpx;
}

.form-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 24rpx 0;
  border-bottom: 1rpx solid #f0f0f0;
}

.form-item:last-child {
  border-bottom: none;
}

.form-label {
  font-size: 28rpx;
  color: #333;
  width: 160rpx;
  flex-shrink: 0;
}

.required {
  color: #e43d33;
}

.form-input {
  flex: 1;
  text-align: right;
  font-size: 28rpx;
  color: #333;
}

.submit-btn {
  width: 100%;
  height: 96rpx;
  line-height: 96rpx;
  background-color: #18bc37;
  color: #ffffff;
  font-size: 34rpx;
  font-weight: bold;
  border-radius: 12rpx;
  border: none;
}

.submit-btn:active {
  opacity: 0.85;
}
</style>
