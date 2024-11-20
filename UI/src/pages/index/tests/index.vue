<template>
    <MLVbox align="center" class="ml-tests">
        <MLHbox style="width: 100%; max-width: 1200px; flex-grow: 1; overflow: hidden; padding-top: 36px;">
            <MLVbox style="width: 250px;">
                <VLabel style="font-size: 1.5rem; margin-bottom: 16px;">Category</VLabel>
                <VCard v-for="x in listTestFilter"
                    :class="[
                        'test-category',
                        {'selected': x.FilterValue === testFilter?.FilterValue}
                    ]"
                    link
                    @click="selectTestFilter(x)"
                >
                    <MLHbox align="center">
                        <VIcon class="test-category-icon" :icon="x.Icon.toString()" />
                        <VLabel class="test-category-text">{{ x.Text }}</VLabel>
                    </MLHbox>
                </VCard>
            </MLVbox>
            
            <div ref="testGrid" class="test-grid" style="flex-grow: 1; overflow-y: auto; margin-left: 24px; padding-right: 24px;">
                <!-- Không có dữ liệu -->
                <MLVbox align="center" v-if="allDataLoaded && listTest.length === 0" style="margin-top: 12px;">
                    <VIcon size="128" icon="mdi-file-document-alert" />
                    <VLabel style="margin-top: 8px;">No test available.</VLabel>
                </MLVbox>

                <!-- Thẻ bài test -->
                <VCard v-for="t in listTest" link class="border-panel test-item" @click="handleTestItemClick(t)">
                    <MLVbox style="height: 100%;">
                        <VLabel class="test-item-number">Test {{ t.TestNumber }}</VLabel>
                        <VLabel class="test-item-date">{{ CommonFunction.formatDate(t.TestDate) }}</VLabel>

                        <MLHbox align="center" class="test-item-prop">
                            <!-- Thời gian -->
                            <MLHbox align="center">
                                <VIcon class="test-item-prop-icon" icon="mdi-clock-outline" />
                                <VLabel class="test-item-prop-info">{{ t.TimeLimit ? CommonFunction.formatTimeBySecond(t.TimeLimit) : 'Unset' }}</VLabel>
                            </MLHbox>

                            <!-- Số câu hỏi -->
                            <MLHbox align="center" style="margin-left: 16px;">
                                <VIcon class="test-item-prop-icon" icon="mdi-format-list-bulleted" />
                                <VLabel class="test-item-prop-info">{{ t.ExerciseCount }} Questions</VLabel>
                            </MLHbox>

                            <!-- Trạng thái -->
                             <span class="test-item-prop-status" :style="{'color': testStatusColor(t)}">{{testStatusName(t)}}</span>
                        </MLHbox>
                    </MLVbox>
                </VCard>

                <!-- Skeleton -->
                <VLazy v-if="!allDataLoaded && existSkeleton" ref="skeleton" :min-height="24" v-model:model-value="showSkeleton" @update:model-value="loadMoreData">
                    <div class="test-grid">
                        <VCard v-for="_ in pageSize" class="border-panel test-item">
                            <MLVbox style="height: 100%;">
                                <!-- Số test -->
                                <VSkeletonLoader :width="108" :height="32" />
                                <!-- Thời gian -->
                                <VSkeletonLoader :width="72" :height="16" style="margin-top: 4px;" />

                                <MLHbox class="test-item-prop">
                                    <!-- Thời gian -->
                                    <VSkeletonLoader :width="72" :height="20" />

                                    <!-- Số câu hỏi -->
                                    <VSkeletonLoader :width="72" :height="20" style="margin-left: 16px;" />

                                    <VSkeletonLoader :width="72" :height="20" style="margin-left: auto;" />
                                </MLHbox>
                            </MLVbox>
                        </VCard>
                    </div>
                </VLazy>
            </div>
        </MLHbox>
    </MLVbox>
</template>

<script lang="ts">
import { EnumTestFilter } from '@/common/Enumeration';
import { Test } from '@/models';

interface TestFilter {
    Icon: String,
    Url: String,
    Text: String,
    Count: number,
    FilterValue: EnumTestFilter
}

export default {
    created() {
        this.selectTestFilter(this.listTestFilter[0]);
    },

    methods: {
        /**
         * Xử lý khi chọn test filter
         */
        selectTestFilter(testFilter:TestFilter) {
            if (this.testFilter === testFilter) {
                return;
            }

            this.allDataLoaded = false;
            this.page = 1;
            this.testFilter = testFilter;

            this.listTest = [];
            this.showSkeleton = true;

            this.loadMoreData();
        },

        /**
         * Load thêm dữ liệu (Infinite scroll)
         */
        async loadMoreData() {
            const currentTestFilter = this.testFilter;

            try {
                const listTestResult:Test[] = await (this as any).$service.TestService.getTestList(this.testFilter?.FilterValue, this.page, this.pageSize);

                //Trong trường hợp chưa load xong API trước mà click chọn sang bộ lọc khác thì không làm gì nữa
                //Vì nếu làm vậy thì bộ lọc sẽ hiển thị khác với dữ liệu trả về
                if (currentTestFilter?.FilterValue !== this.testFilter?.FilterValue) {
                    return;
                }

                //Xoá hẳn component VLazy để tránh trigger event Load dữ liệu
                this.showSkeleton = false;
                this.existSkeleton = false;
                await this.$nextTick();

                //Disable cuộn của list để khi thêm bản ghi mới vào nó không bị cuộn thêm xuống dưới
                const container:any = this.$refs.testGrid;
                container.style.overflowY = 'hidden';

                //Đẩy kết quả bản ghi mới
                this.listTest = [...this.listTest, ...listTestResult];
                if (listTestResult.length < this.pageSize) {
                    this.allDataLoaded = true;
                }

                //Thêm xong thì bật lại cuộn và thêm lại component VLazy (nhưng vẫn ẩn phía dưới)
                this.$nextTick(() => {
                    container.style.overflowY = 'auto';
                    container.scrollTop = Math.min(container.scrollTop, container.scrollHeight - container.clientHeight - 32);

                    this.existSkeleton = true;
                });

                this.page++;
            } catch (e) {
            } finally {
            }
        },

        /**
         * Xử lý khi click vào bài test
         */
        handleTestItemClick(test:Test) {
            this.openTestDetail(test.TestID);
        },

        /**
         * Mở trang chi tiết bài test
         */
        openTestDetail(testID:any) {
            this.$router.push({ name: '//tests/[testID]', params: {
                testID: testID
            } });
        },

        /**
         * Màu chữ trạng thái bài test
         */
        testStatusColor(test:Test) {
            return test.UserAttempt ? (test.UserAttempt.IsFinished ? 'rgb(var(--v-theme-error))' : 'rgb(var(--v-theme-warning))') : 'rgb(var(--v-theme-success))';
        },

        /**
         * Tên trạng thái bài test
         */
        testStatusName(test:Test) {
            return test.UserAttempt ? (test.UserAttempt.IsFinished ? 'Finished' : 'Working') : 'New';
        },
    },

    data() {
        return {
            listTestFilter: <TestFilter[]>[
                {
                    Icon: 'mdi-calendar-clock-outline',
                    Url: 'tests/unfinished',
                    Text: 'Unfinished',
                    Count: 1,
                    FilterValue: this.$enumeration.EnumTestFilter.Unfinished
                },
                {
                    Icon: 'mdi-file-document-multiple-outline',
                    Url: '',
                    Text: 'All',
                    Count: 100,
                    FilterValue: this.$enumeration.EnumTestFilter.All
                }
            ],
            testFilter: <TestFilter|undefined>undefined,

            listTest: <Test[]>[],

            showSkeleton: false,    //Biến này dùng để ẩn hiện component VLazy
            existSkeleton: true,    //Biến này dùng để xoá hẳn component VLazy ra khỏi dom để tránh trigger event load thêm dữ liệu

            page: <number>1,
            pageSize: <number>10,
            allDataLoaded: <boolean>false
        }
    },

    computed: {
        CommonFunction() {
            return this.$commonFunction;
        }
    }
}
</script>

<style lang="scss" scoped>
.ml-tests {
    padding: 24px;
}

.test-category {
    padding: 8px 12px;
    border-radius: 6px;
    margin-bottom: 8px;

    .test-category-icon {
        width: 24px;
        height: 24px;

        &::before {
            opacity: 0.7;
        }
    }

    .test-category-text {
        margin-left: 16px;
        flex-grow: 1;
    }

    .test-category-count {
        margin-left: 16px;
    }

    &.selected {
        background-color: rgb(var(--v-theme-primary));
        color: white;

        .test-category-text, .test-category-icon::before {
            opacity: 1;
        }
    }
}

.test-grid {
    display: grid;
    grid-template-columns: 1fr;
    grid-row-gap: 16px;
    height: fit-content;
    max-height: 100%;

    .test-item {
        min-height: 120px;
        padding: 12px 20px;

        .test-item-number {
            font-weight: bold;
            font-size: var(--big-font-size);
        }

        .test-item-date {
            font-size: 13px;
            font-style: italic;
        }

        .test-item-prop {
            margin-top: auto;

            .test-item-prop-icon {
                width: 24px;
                height: 24px;
                display: flex;
                align-items: center;
                justify-content: center;
                opacity: 0.7;
            }

            .test-item-prop-info {
                font-size: 14px;
                margin-left: 4px;
            }

            .test-item-prop-status {
                margin-left: auto;
            }
        }
    }
}
</style>