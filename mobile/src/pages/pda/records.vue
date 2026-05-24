<template>
  <view class="records-page">
    <!-- 搜索栏 -->
    <view class="search-bar">
      <input
        v-model="searchBarcode"
        placeholder="搜索条形码"
        class="search-input"
        @confirm="onSearch"
      />
      <view class="filter-btn" @click="showFilter = !showFilter">
        <uni-icons type="filter" size="20" color="#666" />
      </view>
    </view>

    <!-- 筛选条件 -->
    <view v-if="showFilter" class="filter-section">
      <text class="filter-label">操作类型</text>
      <view class="filter-tags">
        <uni-tag
          text="全部"
          :type="filterType === '' ? 'primary' : 'default'"
          size="small"
          @click="filterType = ''"
        />
        <uni-tag
          text="入库"
          :type="filterType === '入库' ? 'primary' : 'default'"
          size="small"
          @click="filterType = '入库'"
        />
        <uni-tag
          text="出库"
          :type="filterType === '出库' ? 'primary' : 'default'"
          size="small"
          @click="filterType = '出库'"
        />
        <uni-tag
          text="上架"
          :type="filterType === '上架' ? 'primary' : 'default'"
          size="small"
          @click="filterType = '上架'"
        />
      </view>
    </view>

    <!-- 记录列表 -->
    <scroll-view
      scroll-y
      class="record-list"
      refresher-enabled
      :refresher-triggered="refreshing"
      @refresherrefresh="onRefresh"
      @scrolltolower="onLoadMore"
    >
      <view v-if="records.length === 0 && !loading" class="empty-state">
        <uni-icons type="info" size="48" color="#ccc" />
        <text class="empty-text">暂无扫码记录</text>
      </view>

      <view
        v-for="record in records"
        :key="record.id"
        class="card record-card"
      >
        <view class="record-header">
          <uni-tag
            :text="record.操作类型"
            :type="getTagType(record.操作类型)"
            size="small"
          />
          <text class="record-time">{{ formatTime(record.操作时间) }}</text>
        </view>
        <view class="record-body">
          <view class="record-row">
            <text class="record-label">条形码</text>
            <text class="record-value">{{ record.条形码 }}</text>
          </view>
          <view class="record-row">
            <text class="record-label">数量</text>
            <text class="record-value highlight">{{ record.数量 }}</text>
          </view>
          <view v-if="record.关联单号" class="record-row">
            <text class="record-label">关联单号</text>
            <text class="record-value">{{ record.关联单号 }}</text>
          </view>
          <view v-if="record.操作人" class="record-row">
            <text class="record-label">操作人</text>
            <text class="record-value">{{ record.操作人 }}</text>
          </view>
        </view>
      </view>

      <view v-if="loading" class="loading-tip">
        <uni-load-more status="loading" />
      </view>
      <view v-if="noMore && records.length > 0" class="loading-tip">
        <text class="no-more-text">没有更多了</text>
      </view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { getRecords, type ScanRecord } from '../../api/pda'

const records = ref<ScanRecord[]>([])
const loading = ref(false)
const refreshing = ref(false)
const noMore = ref(false)
const showFilter = ref(false)

const searchBarcode = ref('')
const filterType = ref('')
const pageIndex = ref(1)
const pageSize = 20

onMounted(() => {
  loadRecords(true)
})

watch(filterType, () => {
  loadRecords(true)
})

async function loadRecords(reset = false) {
  if (loading.value) return
  if (reset) {
    pageIndex.value = 1
    noMore.value = false
  }
  if (noMore.value) return

  loading.value = true
  try {
    const result = await getRecords({
      条形码: searchBarcode.value || undefined,
      操作类型: filterType.value || undefined,
      pageIndex: pageIndex.value,
      pageSize
    })
    if (reset) {
      records.value = result.items
    } else {
      records.value = [...records.value, ...result.items]
    }
    if (records.value.length >= result.total) {
      noMore.value = true
    }
    pageIndex.value++
  } catch {
    // 错误已由 request.ts 处理
  } finally {
    loading.value = false
  }
}

function onSearch() {
  loadRecords(true)
}

async function onRefresh() {
  refreshing.value = true
  await loadRecords(true)
  refreshing.value = false
}

function onLoadMore() {
  if (!noMore.value && !loading.value) {
    loadRecords(false)
  }
}

function getTagType(type: string) {
  switch (type) {
    case '入库': return 'success'
    case '出库': return 'warning'
    case '上架': return 'primary'
    default: return 'default'
  }
}

function formatTime(time: string) {
  if (!time) return ''
  const d = new Date(time)
  const pad = (n: number) => n.toString().padStart(2, '0')
  return `${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}`
}
</script>

<style scoped>
.records-page {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background-color: #f5f5f5;
}

.search-bar {
  display: flex;
  align-items: center;
  padding: 20rpx 30rpx;
  background-color: #ffffff;
  gap: 16rpx;
}

.search-input {
  flex: 1;
  height: 72rpx;
  background-color: #f5f5f5;
  border-radius: 36rpx;
  padding: 0 30rpx;
  font-size: 28rpx;
}

.filter-btn {
  width: 72rpx;
  height: 72rpx;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f5f5f5;
  border-radius: 50%;
}

.filter-section {
  background-color: #ffffff;
  padding: 20rpx 30rpx;
  border-top: 1rpx solid #f0f0f0;
}

.filter-label {
  font-size: 24rpx;
  color: #999;
  margin-bottom: 16rpx;
}

.filter-tags {
  display: flex;
  gap: 16rpx;
}

.record-list {
  flex: 1;
  padding: 20rpx 30rpx;
}

.record-card {
  margin-bottom: 20rpx;
}

.record-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 16rpx;
}

.record-time {
  font-size: 24rpx;
  color: #999;
}

.record-body {
  background-color: #f8f9fa;
  border-radius: 12rpx;
  padding: 16rpx 20rpx;
}

.record-row {
  display: flex;
  justify-content: space-between;
  padding: 8rpx 0;
}

.record-label {
  font-size: 26rpx;
  color: #999;
}

.record-value {
  font-size: 26rpx;
  color: #333;
}

.record-value.highlight {
  color: #2979ff;
  font-weight: bold;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 120rpx 0;
}

.empty-text {
  font-size: 28rpx;
  color: #ccc;
  margin-top: 16rpx;
}

.loading-tip {
  padding: 30rpx 0;
  display: flex;
  justify-content: center;
}

.no-more-text {
  font-size: 24rpx;
  color: #ccc;
}
</style>
